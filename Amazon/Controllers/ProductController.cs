using Amazon.Models;
using BLL.Interfaces;
using DAL.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Amazon.Controllers;

public class ProductController : Controller
{
    private const int PageSize = 12;

    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public ProductController(IProductService productService, ICategoryService categoryService,
        AppDbContext db, UserManager<User> userManager)
    {
        _productService  = productService;
        _categoryService = categoryService;
        _db              = db;
        _userManager     = userManager;
    }

    public async Task<IActionResult> Index(int? categoryId, string? search, int page = 1)
    {
        var categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
        ViewData["Categories"] = categories;
        ViewData["Search"]     = search;
        ViewData["CategoryId"] = categoryId;
        ViewData["Page"]       = page;

        bool isSearch   = !string.IsNullOrWhiteSpace(search);
        bool isCategory = categoryId.HasValue;

        if (!isSearch && !isCategory)
            return View((PagedResult<ProductCardViewModel>?)null);

        var query = _db.Products.Include(p => p.Category).AsQueryable();

        if (isSearch)
            query = query.Where(p =>
                p.Name.Contains(search!) || p.Category.Name.Contains(search!));

        if (isCategory)
        {
            var cat = categories.FirstOrDefault(c => c.Id == categoryId);
            ViewData["CategoryName"] = cat?.Name ?? "";
            query = query.Where(p => p.CategoryId == categoryId);
        }

        var total      = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)PageSize);
        var items      = await query
            .OrderBy(p => p.Name)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new ProductCardViewModel
            {
                Id          = p.Id,
                Name        = p.Name,
                Category    = p.Category.Name,
                CategoryId  = p.CategoryId,
                Price       = p.Price,
                Description = p.Description,
                ImageUrl    = p.ImageUrl
            })
            .ToListAsync();

        return View(new PagedResult<ProductCardViewModel>
        {
            Items       = items,
            Page        = page,
            TotalPages  = totalPages,
            TotalItems  = total
        });
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return NotFound();

        var reviews = await _db.Reviews
            .Where(r => r.ProductId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        double? rating     = reviews.Any() ? reviews.Average(r => r.Stars) : null;
        int?    reviewCount = reviews.Any() ? reviews.Count : null;

        var vm = new ProductCardViewModel
        {
            Id          = product.Id,
            Name        = product.Name,
            Category    = product.Category.Name,
            CategoryId  = product.CategoryId,
            Price       = product.Price,
            Description = product.Description,
            ImageUrl    = product.ImageUrl,
            Rating      = rating,
            ReviewCount = reviewCount
        };

        ViewData["Reviews"] = reviews;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(int productId, string body, int stars, string authorName)
    {
        stars = Math.Clamp(stars, 1, 5);
        var userId = _userManager.GetUserId(User);
        string name = authorName?.Trim() is { Length: > 0 } n ? n : "Анонім";

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            name = user?.UserName ?? name;
        }

        _db.Reviews.Add(new Review
        {
            ProductId  = productId,
            UserId     = userId,
            AuthorName = name,
            Body       = body?.Trim() ?? "",
            Stars      = stars,
            CreatedAt  = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = productId });
    }
}

using Amazon.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(int? categoryId, string? search)
    {
        var categories = (await _categoryService.GetAllCategoriesAsync()).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var allProducts = (await _productService.GetAllProductsAsync()).Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category.Name,
                CategoryId = p.CategoryId,
                Price = p.Price,
                Description = p.Description,
                ImageUrl = p.ImageUrl
            }).Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Category.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewData["Search"] = search;
            ViewData["Categories"] = categories;
            return View(allProducts);
        }

        if (categoryId.HasValue)
        {
            var category = categories.FirstOrDefault(c => c.Id == categoryId);
            var products = (await _productService.GetAllProductsAsync())
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category.Name,
                    CategoryId = p.CategoryId,
                    Price = p.Price,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl
                }).ToList();

            ViewData["CategoryId"] = categoryId;
            ViewData["CategoryName"] = category?.Name ?? "";
            ViewData["Categories"] = categories;
            return View(products);
        }

        ViewData["Categories"] = categories;
        return View((List<ProductCardViewModel>?)null);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product is null)
            return NotFound();

        return View(new ProductCardViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category.Name,
            CategoryId = product.CategoryId,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl
        });
    }
}

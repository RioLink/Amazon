using Amazon.Models;
using BLL.Interfaces;
using DAL.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Amazon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService   _productService;
        private readonly IOrderService     _orderService;
        private readonly ICategoryService  _categoryService;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _db;

        public AdminController(
            IProductService productService,
            IOrderService orderService,
            ICategoryService categoryService,
            UserManager<User> userManager,
            IWebHostEnvironment env,
            AppDbContext db)
        {
            _productService  = productService;
            _orderService    = orderService;
            _categoryService = categoryService;
            _userManager     = userManager;
            _env             = env;
            _db              = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = (await _productService.GetAllProductsAsync())
                .Select(p => new ProductAdminViewModel
                {
                    Id       = p.Id,
                    Name     = p.Name,
                    Price    = p.Price,
                    Quantity = p.Quantity
                });
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var imageUrl = await SaveImageAsync(model.ImageFile, model.ImageUrl);
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Додайте зображення або вкажіть URL");
                var cats = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(cats, "Id", "Name");
                return View(model);
            }

            var (success, message) = await _productService.AddProductAsync(
                model.Name, model.Price, model.CategoryId,
                model.Description, imageUrl, model.Quantity, model.OldPrice);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                var cats = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(cats, "Id", "Name");
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var model = new ProductEditViewModel
            {
                Id          = product.Id,
                Name        = product.Name,
                Price       = product.Price,
                OldPrice    = product.OldPrice,
                CategoryId  = product.CategoryId,
                Description = product.Description,
                ImageUrl    = product.ImageUrl,
                Quantity    = product.Quantity
            };
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            var imageUrl = await SaveImageAsync(model.ImageFile, model.ImageUrl);
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Додайте зображення або вкажіть URL");
                var cats = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(cats, "Id", "Name", model.CategoryId);
                return View(model);
            }

            var (success, message) = await _productService.UpdateProductAsync(
                model.Id, model.Name, model.Price, model.CategoryId,
                model.Description, imageUrl, model.Quantity, model.OldPrice);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                var cats = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(cats, "Id", "Name", model.CategoryId);
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(IFormFile? file, string? existingUrl)
        {
            if (file != null && file.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext)) return existingUrl ?? "";

                var dir = Path.Combine(_env.WebRootPath, "uploads", "products");
                Directory.CreateDirectory(dir);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var path = Path.Combine(dir, fileName);
                await using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
                return $"/uploads/products/{fileName}";
            }
            return existingUrl ?? "";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detailed(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var model = new ProductDetailedViewModel
            {
                Id          = product.Id,
                Name        = product.Name,
                Price       = product.Price,
                Category    = product.Category.Name,
                Description = product.Description,
                ImageUrl    = product.ImageUrl,
                Quantity    = product.Quantity
            };
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var rawOrders = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.Date)
                .ToListAsync();

            var orders = rawOrders.Select(o => new OrderViewModel
                {
                    Id            = o.Id,
                    Date          = o.Date,
                    TotalAmount   = o.TotalAmount,
                    Status        = o.Status,
                    UserEmail     = o.User?.Email     ?? "",
                    UserName      = o.User?.UserName  ?? "",
                    FullName      = o.FullName,
                    Phone         = o.Phone,
                    City          = o.City,
                    Address       = o.Address,
                    PostalCode    = o.PostalCode,
                    PaymentMethod = o.PaymentMethod
                });

            var since = DateTime.UtcNow.AddDays(-29).Date;
            var dailyRevenue = await _db.Orders
                .Where(o => o.Date >= since && o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.Date.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount) })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var labels  = Enumerable.Range(0, 30).Select(i => since.AddDays(i).ToString("dd.MM")).ToList();
            var dataMap = dailyRevenue.ToDictionary(x => x.Date.ToString("dd.MM"), x => x.Revenue);
            var values  = labels.Select(l => dataMap.ContainsKey(l) ? dataMap[l] : 0m).ToList();

            ViewData["ChartLabels"] = System.Text.Json.JsonSerializer.Serialize(labels);
            ViewData["ChartValues"] = System.Text.Json.JsonSerializer.Serialize(values);

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            await _orderService.ChangeOrderStatusAsync(id, status);
            return RedirectToAction(nameof(Orders));
        }


        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(string name)
        {
            var (success, message) = await _categoryService.CreateAsync(name);
            if (!success) { ModelState.AddModelError("", message); return View(); }
            TempData["Success"] = "Категорію успішно створено!";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string name)
        {
            var (success, message) = await _categoryService.UpdateAsync(id, name);
            if (!success)
            {
                ModelState.AddModelError("", message);
                var category = await _categoryService.GetByIdAsync(id);
                return View(category);
            }
            TempData["Success"] = "Категорію оновлено!";
            return RedirectToAction(nameof(Categories));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var (success, message) = await _categoryService.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Categories));
        }


        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .Include(u => u.Addresses)
                .ToListAsync();

            var models = new List<UserAdminViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                models.Add(new UserAdminViewModel
                {
                    Id           = u.Id,
                    UserName     = u.UserName ?? "",
                    Email        = u.Email ?? "",
                    AvatarPath   = u.AvatarPath,
                    Roles        = roles.ToList(),
                    AddressCount = u.Addresses.Count,
                    LockoutEnd   = u.LockoutEnd
                });
            }
            return View(models);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"Користувача {user.UserName} розблоковано.";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                await _userManager.SetLockoutEnabledAsync(user, true);
                TempData["Success"] = $"Користувача {user.UserName} заблоковано.";
            }
            return RedirectToAction(nameof(Users));
        }
    }
}

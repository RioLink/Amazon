using Amazon.Models;
using BLL.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Amazon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService  _productService;
        private readonly IOrderService    _orderService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<User> _userManager;

        public AdminController(
            IProductService productService,
            IOrderService orderService,
            ICategoryService categoryService,
            UserManager<User> userManager)
        {
            _productService  = productService;
            _orderService    = orderService;
            _categoryService = categoryService;
            _userManager     = userManager;
        }

        // ══════════════════════════════════════════════════════
        // PRODUCTS
        // ══════════════════════════════════════════════════════

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
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            var (success, message) = await _productService.AddProductAsync(
                model.Name, model.Price, model.CategoryId,
                model.Description, model.ImageUrl, model.Quantity);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
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
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            var (success, message) = await _productService.UpdateProductAsync(
                model.Id, model.Name, model.Price, model.CategoryId,
                model.Description, model.ImageUrl, model.Quantity);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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

        // ══════════════════════════════════════════════════════
        // ORDERS
        // ══════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = (await _orderService.GetAllOrdersAsync())
                .Select(o => new OrderViewModel
                {
                    Id          = o.Id,
                    Date        = o.Date,
                    TotalAmount = o.TotalAmount,
                    Status      = o.Status
                });
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            await _orderService.ChangeOrderStatusAsync(id, status);
            return RedirectToAction(nameof(Orders));
        }

        // ══════════════════════════════════════════════════════
        // CATEGORIES
        // ══════════════════════════════════════════════════════

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
            if (!success)
            {
                ModelState.AddModelError("", message);
                return View();
            }
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

        // ══════════════════════════════════════════════════════
        // USERS
        // ══════════════════════════════════════════════════════

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
                    Id         = u.Id,
                    UserName   = u.UserName ?? "",
                    Email      = u.Email ?? "",
                    AvatarPath = u.AvatarPath,
                    Roles      = roles.ToList(),
                    AddressCount = u.Addresses.Count,
                    LockoutEnd = u.LockoutEnd
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
                // Unlock
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success"] = $"Користувача {user.UserName} розблоковано.";
            }
            else
            {
                // Lock for 100 years
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                await _userManager.SetLockoutEnabledAsync(user, true);
                TempData["Success"] = $"Користувача {user.UserName} заблоковано.";
            }
            return RedirectToAction(nameof(Users));
        }
    }
}

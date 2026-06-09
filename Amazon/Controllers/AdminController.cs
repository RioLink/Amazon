using Amazon.Models;
using BLL.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Amazon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;

        public AdminController(IProductService productService, IOrderService orderService, ICategoryService categoryService)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = (await _productService.GetAllProductsAsync()).Select(p => new ProductAdminViewModel { Id = p.Id, Name = p.Name, Price = p.Price, Quantity = p.Quantity });
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
            var (success, message) = await _productService.AddProductAsync(model.Name, model.Price, model.CategoryId, model.Description, model.ImageUrl, model.Quantity);
            
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
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Quantity = product.Quantity
            };

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            var (success, message) = await _productService.UpdateProductAsync(model.Id, model.Name, model.Price, model.CategoryId, model.Description, model.ImageUrl, model.Quantity);
            
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
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Quantity = product.Quantity
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orderViewModels = (await _orderService.GetAllOrdersAsync()).Select(o => new OrderViewModel
            {
                Id = o.Id,
                Date = o.Date,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            });
            return View(orderViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            await _orderService.ChangeOrderStatusAsync(id, status);
            return RedirectToAction(nameof(Orders));
        }
    }
}
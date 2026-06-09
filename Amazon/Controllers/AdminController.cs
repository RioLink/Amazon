using AmazonMVC.BLL.Services;
using BLL.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
            private readonly IProductService _productService;
            private readonly IOrderService _orderService;
            public AdminController(IProductService productService, IOrderService orderService)
            {
                _productService = productService;
                _orderService = orderService;
            }


            public async Task<IActionResult> Index()
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products); 
            }

            public IActionResult Create() => View();


            [HttpPost]
            public async Task<IActionResult> Create(Product product)
            {
                if (ModelState.IsValid)
                {
                    await _productService.AddProductAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                return View(product);
            }

            
            public async Task<IActionResult> Delete(int id)
            {
                await _productService.DeleteProductAsync(id);
                return RedirectToAction(nameof(Index));
            }

        //public async Task<IActionResult> Orders()
        //{
        //    var orders = await _orderService.GetAllOrdersAsync(); 
        //    return View(orders);
        //}
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            // Временно добавь эту строку для проверки:
            if (orders == null || !orders.Any())
            {
                ViewBag.Message = "База заказов пуста или не подключена!";
            }

            return View(orders);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
            {
                await _orderService.ChangeOrderStatusAsync(id, status);
                return RedirectToAction(nameof(Orders));
            }
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}

using Amazon.Models;
using BLL.Interfaces;
using BLL.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<User> _userManager;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, UserManager<User> userManager, ICartService cartService)
        {
            _orderService = orderService;
            _userManager = userManager;
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var items = await _cartService.GetCartByUserIdAsync(userId);
            await _orderService.CreateOrderAsync(userId, items);

            return RedirectToAction("Index", "Home");
        }
    }
}

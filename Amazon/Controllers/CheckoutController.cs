using Amazon.Models;
using Amazon.Services;
using BLL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers;

public class CheckoutController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly UserManager<User> _userManager;

    public CheckoutController(ICartService cartService, IOrderService orderService, UserManager<User> userManager)
    {
        _cartService = cartService;
        _orderService = orderService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        List<CartItemViewModel> viewModel;

        if (userId != null)
        {
            var cartItems = await _cartService.GetCartByUserIdAsync(userId);
            viewModel = cartItems.Select(c => new CartItemViewModel
            {
                ProductId   = c.ProductId,
                ProductName = c.Product?.Name ?? "Товар",
                Quantity    = c.Quantity,
                Price       = c.Product?.Price ?? 0
            }).ToList();
        }
        else
        {
            viewModel = GuestCartService.GetCart(HttpContext.Session);
        }

        if (viewModel.Count == 0)
            return RedirectToAction("Index", "Cart");

        return View(viewModel);
    }

    [HttpPost]
    [Authorize]  // Оформлення замовлення — тільки для авторизованих
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(string fullName, string email, string phone,
        string city, string postal, string address, string payment)
    {
        var userId = _userManager.GetUserId(User)!;
        var cartItems = await _cartService.GetCartByUserIdAsync(userId);

        if (!cartItems.Any())
            return RedirectToAction("Index", "Cart");

        await _orderService.CreateOrderAsync(userId, cartItems);
        await _cartService.ClearCartAsync(userId);

        TempData["OrderSuccess"] = "true";
        TempData["OrderAddress"] = $"{city}, {address}";
        return RedirectToAction("Confirm");
    }

    public IActionResult Confirm()
    {
        if (TempData["OrderSuccess"]?.ToString() != "true")
            return RedirectToAction("Index", "Home");

        ViewData["OrderAddress"] = TempData["OrderAddress"];
        return View();
    }
}

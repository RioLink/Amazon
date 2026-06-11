using Amazon.Models;
using Amazon.Services;
using BLL.Interfaces;
using DAL.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Amazon.Controllers;

public class CheckoutController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _db;

    public CheckoutController(ICartService cartService, IOrderService orderService, UserManager<User> userManager, AppDbContext db)
    {
        _cartService = cartService;
        _orderService = orderService;
        _userManager = userManager;
        _db = db;
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

        if (userId != null)
        {
            var addresses = await _db.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ToListAsync();
            ViewData["SavedAddresses"] = addresses;
        }

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

        await _orderService.CreateOrderAsync(userId, cartItems,
            fullName, phone, city, address, postal, payment);
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

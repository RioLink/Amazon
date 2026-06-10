using Amazon.Models;
using BLL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly UserManager<User> _userManager;

    public CartController(ICartService cartService, UserManager<User> userManager)
    {
        _cartService = cartService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge(); // Если вдруг не авторизован

        var cartItems = await _cartService.GetCartByUserIdAsync(userId);

        var viewModel = cartItems.Select(c => new CartItemViewModel
        {
            ProductId = c.ProductId,
            ProductName = c.Product?.Name ?? "Товар",
            Quantity = c.Quantity,
            Price = c.Product?.Price ?? 0
        }).ToList();

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddToCartViewModel model)
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var result = await _cartService.AddToCartAsync(userId, model.ProductId, model.Quantity);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ОШИБКА: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"СТЭК: {ex.StackTrace}");
        }
        
        var referer = Request.Headers.Referer.ToString();
        
        if (!string.IsNullOrEmpty(referer))
            return Redirect(referer);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int productId)
    {
        var userId = _userManager.GetUserId(User);
        await _cartService.RemoveFromCartAsync(userId, productId);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Json(new { count = 0 });

        int count = await _cartService.GetCartSizeByUserIdAsync(userId);
        return Json(new { count });
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        var userId = _userManager.GetUserId(User);
        await _cartService.ClearCartAsync(userId);
        return RedirectToAction(nameof(Index));
    }
}
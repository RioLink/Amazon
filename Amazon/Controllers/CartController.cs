using Amazon.Models;
using Amazon.Services;
using BLL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IProductService _productService;
    private readonly UserManager<User> _userManager;

    public CartController(ICartService cartService, IProductService productService, UserManager<User> userManager)
    {
        _cartService = cartService;
        _productService = productService;
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
                Price       = c.Product?.Price ?? 0,
                ImageUrl    = c.Product?.ImageUrl
            }).ToList();
        }
        else
        {
            viewModel = GuestCartService.GetCart(HttpContext.Session);
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddToCartViewModel model)
    {
        var userId = _userManager.GetUserId(User);

        if (userId != null)
        {
            var result = await _cartService.AddToCartAsync(userId, model.ProductId, model.Quantity);
            if (!result.Success)
                TempData["Error"] = result.Message;
        }
        else
        {
            var product = await _productService.GetProductByIdAsync(model.ProductId);
            if (product != null)
                GuestCartService.Add(HttpContext.Session, product.Id, product.Name, product.Price, model.Quantity, product.ImageUrl);
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
        if (userId != null)
            await _cartService.RemoveFromCartAsync(userId, productId);
        else
            GuestCartService.Remove(HttpContext.Session, productId);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Clear()
    {
        var userId = _userManager.GetUserId(User);
        if (userId != null)
            await _cartService.ClearCartAsync(userId);
        else
            GuestCartService.Clear(HttpContext.Session);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQtyRequest req)
    {
        var userId = _userManager.GetUserId(User);
        if (userId != null)
        {
            var cartItems = await _cartService.GetCartByUserIdAsync(userId);
            var item = cartItems.FirstOrDefault(c => c.ProductId == req.ProductId);
            if (item != null)
            {
                int newQty = item.Quantity + req.Delta;
                if (newQty <= 0)
                    await _cartService.RemoveFromCartAsync(userId, req.ProductId);
                else
                    await _cartService.AddToCartAsync(userId, req.ProductId, req.Delta);
            }
        }
        else
        {
            if (req.Delta > 0)
                GuestCartService.Add(HttpContext.Session, req.ProductId, "", 0, req.Delta);
            else
                GuestCartService.Remove(HttpContext.Session, req.ProductId);
        }

        var count = userId != null
            ? await _cartService.GetCartSizeByUserIdAsync(userId)
            : GuestCartService.GetCount(HttpContext.Session);

        return Json(new { success = true, cartCount = count });
    }

    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        var userId = _userManager.GetUserId(User);
        int count;
        if (userId != null)
            count = await _cartService.GetCartSizeByUserIdAsync(userId);
        else
            count = GuestCartService.GetCount(HttpContext.Session);

        return Json(new { count });
    }
}

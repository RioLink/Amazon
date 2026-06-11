using DAL.Data;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Amazon.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<User> _userManager;

    public WishlistController(AppDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int productId)
    {
        var userId = _userManager.GetUserId(User)!;
        var existing = await _db.Wishlist
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        bool added;
        if (existing != null)
        {
            _db.Wishlist.Remove(existing);
            added = false;
        }
        else
        {
            _db.Wishlist.Add(new WishlistItem { UserId = userId, ProductId = productId });
            added = true;
        }
        await _db.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { added });

        return Redirect(Request.Headers.Referer.ToString());
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var items = await _db.Wishlist
            .Where(w => w.UserId == userId)
            .Include(w => w.Product).ThenInclude(p => p.Category)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();
        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> GetIds()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Json(new int[0]);
        var ids = await _db.Wishlist
            .Where(w => w.UserId == userId)
            .Select(w => w.ProductId)
            .ToListAsync();
        return Json(ids);
    }
}

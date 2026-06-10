using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;

namespace Amazon.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _db;

    public OrdersController(UserManager<User> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Auth");

        var orders = await _db.Orders
            .Where(o => o.UserId == user.Id)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        return View(orders);
    }
}

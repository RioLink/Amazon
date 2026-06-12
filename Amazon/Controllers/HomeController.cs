using Amazon.Models;
using BLL.Services;
using BLL.Interfaces;
using DAL.Data;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Amazon.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _db;

        public HomeController(IProductService productService, AppDbContext db)
        {
            _productService = productService;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var popular = (await _productService.GetMostPopularProductsAsync()).Select(p => new ProductCardViewModel {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category.Name,
                CategoryId = p.CategoryId,
                Price = p.Price,
                OldPrice = p.OldPrice,
                Description = p.Description,
                ImageUrl = p.ImageUrl
            }).ToList();

            // 4 товари з різних категорій, не Техніка — по одному з кожної
            var otherProducts = (await _db.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Name != "Техніка")
                .OrderBy(p => p.Id)
                .ToListAsync())
                .GroupBy(p => p.CategoryId)
                .Select(g => g.First())
                .OrderBy(_ => Guid.NewGuid())   // рандом вже в пам'яті
                .Take(4)
                .Select(p => new ProductCardViewModel {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category.Name,
                    CategoryId = p.CategoryId,
                    Price = p.Price,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl
                })
                .ToList();

            ViewData["OtherProducts"] = otherProducts;

            var categories = await _db.Categories
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
            ViewData["Categories"] = categories;

            return View(popular);
        }

        public IActionResult Privacy() => View();
        public IActionResult PrivacyPolicy() => View();
        public IActionResult Terms() => View();
        public IActionResult Disclaimer() => View();
        public IActionResult AboutUs() => View();
        public IActionResult Careers() => View();

        [HttpGet]
        public IActionResult SetLanguage(string culture, string? returnUrl)
        {
            var allowedCultures = new HashSet<string> { "uk", "en" };
            if (!allowedCultures.Contains(culture))
                culture = "uk";

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires  = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                }
            );

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult StatusCode(int code)
        {
            ViewData["StatusCode"] = code;
            return View();
        }
    }
}

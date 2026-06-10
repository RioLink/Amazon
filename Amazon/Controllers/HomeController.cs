using Amazon.Models;
using BLL.Services;
using BLL.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Amazon.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService) 
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = (await _productService.GetMostPopularProductsAsync()).Select(p => new ProductCardViewModel { 
                Id = p.Id,  
                Name = p.Name, 
                Category = p.Category.Name, 
                Price = p.Price, 
                Description = p.Description, 
                ImageUrl = p.ImageUrl 
            }).ToList();

            return View(products);
        }

        public IActionResult Privacy() => View();
        public IActionResult PrivacyPolicy() => View();
        public IActionResult Terms() => View();
        public IActionResult Disclaimer() => View();
        public IActionResult AboutUs() => View();

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
    }
}

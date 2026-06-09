using Amazon.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Amazon.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(DemoCatalog.Products);
        }

        public IActionResult Privacy() => View();
        public IActionResult PrivacyPolicy() => View();
        public IActionResult Terms() => View();
        public IActionResult Disclaimer() => View();
        public IActionResult AboutUs() => View();

        /// <summary>
        /// Switches the UI language by writing the culture cookie and redirecting back.
        /// GET /Home/SetLanguage?culture=en&amp;returnUrl=/
        /// </summary>
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

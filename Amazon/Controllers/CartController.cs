using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

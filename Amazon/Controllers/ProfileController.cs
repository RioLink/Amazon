using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers;

public class ProfileController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

using Amazon.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public ActionResult Index() 
        {
            return View();
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel user) 
        {
            var (success, message) = await _authService.RegisterAsync(user.Username, user.Password, user.ConfirmPassword);

            if ( success)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", message);

            return View(user);
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var (success, message) = await _authService.LoginAsync(model.Username, model.Password);

            if (success)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", message);
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Auth");
        }
    }
}

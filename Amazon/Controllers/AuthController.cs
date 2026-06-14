using Amazon.Models;
using Amazon.Services;
using BLL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Amazon.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService      _authService;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService     _emailService;
        private readonly ICartService      _cartService;

        public AuthController(IAuthService authService,
                              UserManager<User> userManager,
                              IEmailService emailService,
                              ICartService cartService)
        {
            _authService  = authService;
            _userManager  = userManager;
            _emailService = emailService;
            _cartService  = cartService;
        }

        [HttpGet]
        public ActionResult Index() => View();

        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            if (!ModelState.IsValid) return View(user);

            var (success, message) = await _authService.RegisterAsync(
                user.Username, user.Email, user.Password, user.ConfirmPassword);

            if (success)
            {
                var guestCart = GuestCartService.GetCart(HttpContext.Session);
                if (guestCart.Count > 0)
                {
                    var registeredUser = await _userManager.FindByNameAsync(user.Username);
                    if (registeredUser != null)
                    {
                        foreach (var item in guestCart)
                            await _cartService.AddToCartAsync(registeredUser.Id, item.ProductId, item.Quantity);
                    }
                    GuestCartService.Clear(HttpContext.Session);
                }

                return RedirectToAction("Index", "Home"); 
            }

            ModelState.AddModelError("", message);
            return View(user);
        }

        [HttpGet]
        public ActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message) = await _authService.LoginAsync(model.Username, model.Password);

            if (success)
            {
                var guestCart = GuestCartService.GetCart(HttpContext.Session);
                if (guestCart.Count > 0)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null)
                    {
                        foreach (var item in guestCart)
                            await _cartService.AddToCartAsync(user.Id, item.ProductId, item.Quantity);
                    }
                    GuestCartService.Clear(HttpContext.Session);
                }

                var returnUrl = Request.Query["ReturnUrl"].ToString();
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Auth");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Введіть email.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email.Trim());

            if (user != null)
            {
                var token      = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetUrl   = Url.Action("ResetPassword", "Auth",
                    new { token, email = user.Email }, Request.Scheme)!;

                var body = $@"
<div style='font-family:sans-serif;max-width:520px;margin:auto;padding:24px'>
  <h2 style='color:#131921'>Відновлення паролю — Amazon ITStep</h2>
  <p>Ви (або хтось інший) запросили скидання паролю для акаунту <b>{user.UserName}</b>.</p>
  <p>Натисніть кнопку нижче, щоб встановити новий пароль. Посилання дійсне <b>24 години</b>.</p>
  <a href='{resetUrl}'
     style='display:inline-block;margin:16px 0;padding:12px 28px;background:#131921;
            color:#fff;text-decoration:none;border-radius:8px;font-weight:700;font-size:15px'>
    Скинути пароль →
  </a>
  <p style='color:#888;font-size:12px'>Якщо ви не запитували скидання — просто проігноруйте цей лист.</p>
  <hr style='border:none;border-top:1px solid #eee;margin:20px 0'/>
  <p style='color:#aaa;font-size:11px'>© {DateTime.Now.Year} Amazon ITStep. Навчальний проєкт.</p>
</div>";

                await _emailService.SendAsync(user.Email!, "Відновлення паролю — Amazon ITStep", body);
            }

            TempData["ForgotEmail"] = email.Trim();
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult ResetPassword(string? token, string? email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            ViewData["Token"] = token;
            ViewData["Email"] = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, string email,
            string password, string confirmPassword)
        {
            ViewData["Token"] = token;
            ViewData["Email"] = email;

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            { ModelState.AddModelError("", "Мінімум 8 символів."); return View(); }

            if (password != confirmPassword)
            { ModelState.AddModelError("", "Паролі не збігаються."); return View(); }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            { TempData["ResetSuccess"] = true; return RedirectToAction("ResetPasswordConfirmation"); }

            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (result.Succeeded)
            {
                TempData["ResetSuccess"] = true;
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var e in result.Errors)
                ModelState.AddModelError("", e.Description);

            return View();
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();
    }
}

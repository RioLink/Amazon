using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;

namespace Amazon.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext      _db;
    private readonly IWebHostEnvironment _env;

    public ProfileController(UserManager<User> userManager, AppDbContext db, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _db          = db;
        _env         = env;
    }

    // ── Index ────────────────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

        ViewData["UserEmail"]    = user?.Email ?? "";
        ViewData["AvatarPath"]   = user?.AvatarPath ?? "";
        ViewData["Addresses"]    = user?.Addresses.OrderByDescending(a => a.IsDefault).ThenBy(a => a.CreatedAt).ToList()
                                   ?? new List<Address>();
        return View();
    }

    // ── Upload Avatar ─────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile avatar)
    {
        if (avatar == null || avatar.Length == 0)
            return Json(new { success = false, message = "Файл не вибрано." });

        if (avatar.Length > 2 * 1024 * 1024)
            return Json(new { success = false, message = "Максимальний розмір: 2 МБ." });

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var ext = Path.GetExtension(avatar.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
            return Json(new { success = false, message = "Дозволено: JPG, PNG, WEBP, GIF." });

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false, message = "Користувача не знайдено." });

        // Delete old avatar file
        if (!string.IsNullOrEmpty(user.AvatarPath))
        {
            var oldPath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }

        // Save new file
        var fileName  = $"{user.Id}{ext}";
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsDir);
        var filePath  = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await avatar.CopyToAsync(stream);

        user.AvatarPath = $"/uploads/avatars/{fileName}";
        await _userManager.UpdateAsync(user);

        return Json(new { success = true, url = user.AvatarPath });
    }

    // ── Change Email ──────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeEmail(string NewEmail, string CurrentPassword)
    {
        if (string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(CurrentPassword))
            return Json(new { success = false, message = "Заповніть всі поля." });

        if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(NewEmail))
            return Json(new { success = false, message = "Невірний формат email." });

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false, message = "Користувача не знайдено." });

        // Verify current password
        var pwOk = await _userManager.CheckPasswordAsync(user, CurrentPassword);
        if (!pwOk) return Json(new { success = false, message = "Невірний поточний пароль." });

        // Check if email already taken
        var existing = await _userManager.FindByEmailAsync(NewEmail.Trim());
        if (existing != null && existing.Id != user.Id)
            return Json(new { success = false, message = "Ця пошта вже використовується." });

        user.Email           = NewEmail.Trim();
        user.NormalizedEmail = NewEmail.Trim().ToUpperInvariant();
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Json(new { success = true, email = user.Email });

        var error = string.Join(" ", result.Errors.Select(e => e.Description));
        return Json(new { success = false, message = error });
    }

    // ── Change Password ───────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmNewPassword)
    {
        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
            return Json(new { success = false, message = "Заповніть всі поля." });

        if (NewPassword != ConfirmNewPassword)
            return Json(new { success = false, message = "Паролі не збігаються." });

        if (NewPassword.Length < 8)
            return Json(new { success = false, message = "Мінімум 8 символів." });

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false, message = "Користувача не знайдено." });

        var result = await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
        if (result.Succeeded)
            return Json(new { success = true });

        var error = string.Join(" ", result.Errors.Select(e => e.Description));
        return Json(new { success = false, message = error });
    }

    // ── Add Address ───────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAddress(string FullName, string Phone, string City,
        string Street, string Building, string? Apartment, string? PostalCode, bool IsDefault)
    {
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Phone) ||
            string.IsNullOrWhiteSpace(City)     || string.IsNullOrWhiteSpace(Street) ||
            string.IsNullOrWhiteSpace(Building))
            return Json(new { success = false, message = "Заповніть обов'язкові поля." });

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false, message = "Користувача не знайдено." });

        // Max 5 addresses
        var count = await _db.Addresses.CountAsync(a => a.UserId == user.Id);
        if (count >= 5)
            return Json(new { success = false, message = "Максимум 5 адрес." });

        // If new address is default — reset others
        if (IsDefault)
            await _db.Addresses.Where(a => a.UserId == user.Id && a.IsDefault)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));

        var address = new Address
        {
            UserId     = user.Id,
            FullName   = FullName.Trim(),
            Phone      = Phone.Trim(),
            City       = City.Trim(),
            Street     = Street.Trim(),
            Building   = Building.Trim(),
            Apartment  = Apartment?.Trim(),
            PostalCode = PostalCode?.Trim(),
            IsDefault  = IsDefault || count == 0, // first address is always default
            CreatedAt  = DateTime.UtcNow
        };

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();

        return Json(new { success = true, id = address.Id, isDefault = address.IsDefault });
    }

    // ── Delete Address ────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
        if (address == null) return Json(new { success = false, message = "Адресу не знайдено." });

        bool wasDefault = address.IsDefault;
        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync();

        // If deleted address was default — set first remaining as default
        if (wasDefault)
        {
            var first = await _db.Addresses.Where(a => a.UserId == user.Id).OrderBy(a => a.CreatedAt).FirstOrDefaultAsync();
            if (first != null) { first.IsDefault = true; await _db.SaveChangesAsync(); }
        }

        return Json(new { success = true });
    }

    // ── Set Default Address ───────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetDefaultAddress(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Json(new { success = false });

        await _db.Addresses.Where(a => a.UserId == user.Id && a.IsDefault)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false));

        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
        if (address == null) return Json(new { success = false });

        address.IsDefault = true;
        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }
}

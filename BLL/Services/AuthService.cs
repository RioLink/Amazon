using Domain.Enums;
using BLL.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool success, string message)> RegisterAsync(string username, string email, string password, string confirmPassword)
        {
            if (password.Contains(" "))
                return (false, "Пароль не може містити пробіли.");

            if (password != confirmPassword)
                return (false, "Паролі не збігаються.");

            // Check if email already taken
            var existingEmail = await _userManager.FindByEmailAsync(email);
            if (existingEmail != null)
                return (false, "Ця пошта вже використовується.");

            var user = new User { UserName = username, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
                await _signInManager.SignInAsync(user, isPersistent: false);
                return (true, string.Empty);
            }

            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, message);
        }

        public async Task<(bool success, string message)> LoginAsync(string usernameOrEmail, string password)
        {
            // Support login by email or username
            string username = usernameOrEmail;
            if (usernameOrEmail.Contains('@'))
            {
                var userByEmail = await _userManager.FindByEmailAsync(usernameOrEmail);
                if (userByEmail == null)
                    return (false, "Невірний логін/пошта або пароль.");
                username = userByEmail.UserName ?? usernameOrEmail;
            }

            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
                return (true, string.Empty);

            return (false, "Невірний логін/пошта або пароль.");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}

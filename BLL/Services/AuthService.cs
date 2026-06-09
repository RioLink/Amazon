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

        public async Task<(bool success, string message)> RegisterAsync(string username, string password, string confirmPassword)
        {
            if (password.Contains(" "))
            {
                return (false, "Password cannot contain spaces.");
            }

            if (password != confirmPassword)
            {
                return (false, "Passwords do not match.");
            }

            var user = new User { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if(result.Succeeded) 
            {
                await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
                await _signInManager.SignInAsync(user, isPersistent: false);
                return (true, string.Empty);
            }

            var message = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, message);
        }

        public async Task<(bool success, string message)> LoginAsync(string username, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return (true, string.Empty);
            }
            return (false, "Incorrect password or username.");
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}

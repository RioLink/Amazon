using BLL.DTOs;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        public async Task<bool> RegisterAsync(RegisterDto model)
        {
            // Логику регистрации
            return false;
        }

        public async Task<bool> LoginAsync(LoginDto model)
        {
            // Логику входа 
            return false;
        }

        public async Task LogoutAsync()
        {
            // Логику выхода 
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<(bool success, string message)> RegisterAsync(string username, string password, string confirmPassword);
        Task<(bool success, string message)> LoginAsync(string username, string password);
        Task LogoutAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto model);
        Task<bool> LoginAsync(LoginDto model);
        Task LogoutAsync();
    }
}

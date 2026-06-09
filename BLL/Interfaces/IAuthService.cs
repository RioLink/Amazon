using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;

namespace BLL.Interfaces
{
    public interface IAuthService
    {
        //Task<bool> RegisterAsync(Register model);
        //Task<bool> LoginAsync(Login model);
        Task LogoutAsync();
    }
}

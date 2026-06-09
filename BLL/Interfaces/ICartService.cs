using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTOs;
namespace BLL.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int productId);
        Task<CartDto> GetCartByUserIdAsync(string userId);
    }
}

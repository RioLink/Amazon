using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;
namespace BLL.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task RemoveFromCartAsync(string userId, int productId);
        Task<CartItem> GetCartByUserIdAsync(string userId);
    }
}

using BLL.DTOs;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class CartService : ICartService
    {
        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            // Логика добавления в корзину
        }

        public async Task RemoveFromCartAsync(string userId, int productId)
        {
            // Логика удаления из корзины
        }

        public async Task<CartDto> GetCartByUserIdAsync(string userId)
        {
            return new CartDto();
        }
    }
}

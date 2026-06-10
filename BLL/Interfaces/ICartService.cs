using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartItem>> GetCartByUserIdAsync(string userId);
        Task<int> GetCartSizeByUserIdAsync(string userId);
        Task<(bool Success, string Message)> AddToCartAsync(string userId, int productId, int quantity);
        Task<(bool Success, string Message)> RemoveFromCartAsync(string userId, int productId);
        //Очистка
        Task ClearCartAsync(string userId);
    }
}
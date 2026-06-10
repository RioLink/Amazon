using BLL.Interfaces;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IGenericRepository<Product> _productRepo;

        public CartService(ICartRepository cartRepo, IGenericRepository<Product> productRepo)
        {
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<CartItem>> GetCartByUserIdAsync(string userId)
        {
            var allItems = await _cartRepo.GetAllAsync();
            var userItems = allItems.Where(x => x.UserId == userId).ToList();


            foreach (var item in userItems)
            {
                item.Product = await _productRepo.GetByIdAsync(item.ProductId);
            }
            return userItems;
        }

        public async Task<(bool Success, string Message)> AddToCartAsync(string userId, int productId, int quantity)
        {
            //if (quantity <= 0)
            //    return (false, "Кількість має бути більше 0");

            var product = await _productRepo.GetByIdAsync(productId);
            if (product == null)
                return (false, "Товар не знайдено");

            if (product.Quantity < quantity)
                return (false, "На складі недостатньо товару");

            // Логика добавления
            var items = await _cartRepo.GetAllAsync();
            var existingItem = items.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartRepo.UpdateAsync(existingItem);
            }
            else
            {
                await _cartRepo.AddAsync(new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            return (true, string.Empty);
        }

        public async Task<(bool Success, string Message)> RemoveFromCartAsync(string userId, int productId)
        {
            var items = await _cartRepo.GetAllAsync();
            var item = items.FirstOrDefault(c => c.UserId == userId && c.ProductId == productId);

            if (item == null)
                return (false, "Товар не знайдено в корзині");

            await _cartRepo.DeleteAsync(item.Id);
            return (true, string.Empty);
        }


        public async Task ClearCartAsync(string userId)
        {
            var items = await _cartRepo.GetAllAsync();
            var userItems = items.Where(c => c.UserId == userId).ToList();

            foreach (var item in userItems)
            {
                await _cartRepo.DeleteAsync(item.Id);
            }
        }

        public async Task<int> GetCartSizeByUserIdAsync(string userId) 
        {
            return await _cartRepo.CountByUserIdAsync(userId);
        }
    }
}
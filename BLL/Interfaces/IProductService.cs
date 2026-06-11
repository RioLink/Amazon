using Domain.Entities;

namespace BLL.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetMostPopularProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<(bool Success, string Message)> AddProductAsync(string name, decimal price, int categoryId, string description, string imageUrl, int quantity, decimal? oldPrice = null);
        Task<(bool Success, string Message)> UpdateProductAsync(int id, string name, decimal price, int categoryId, string description, string imageUrl, int quantity, decimal? oldPrice = null);
        Task DeleteProductAsync(int id);
    }
}
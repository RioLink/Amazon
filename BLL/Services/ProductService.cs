using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.Interfaces;
using Domain.Entities;
using DAL.Repositories.Interfaces; // Убедись, что этот неймспейс доступен

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _repo;

        public ProductService(IGenericRepository<Product> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // Убираем всё, что было между <<<<<<< и >>>>>>>, оставляем только это:
            return await _repo.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _repo.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _repo.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
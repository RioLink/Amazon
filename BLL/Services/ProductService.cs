
using BLL.DTOs;
using BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProductService : IProductService
    {
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            // Тут будем подтягивать  товары из базк но пока офф
            return new List<ProductDto>();
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            // Тут буде запит до бази — поки повертаємо порожній об'єкт
            return new ProductDto();
        }
    }
}
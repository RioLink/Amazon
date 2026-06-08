
using BLL.DTOs;
using BLL.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmazonMVC.BLL.Services
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
            return null;
        }
    }
}
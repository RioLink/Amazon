using System;
using System.Collections.Generic;
using System.Text;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto> GetProductByIdAsync(int id);
    }
}

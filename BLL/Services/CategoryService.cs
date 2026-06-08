using BLL.DTOs;
using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return new List<CategoryDto>();
        }
    }
}

using BLL.Interfaces;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _repo;

        public CategoryService(IGenericRepository<Category> repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _repo.GetAllAsync();
        }
    }
}

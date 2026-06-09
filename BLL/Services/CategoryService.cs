using BLL.Interfaces;
using DAL.Data;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IGenericRepository<Category> _repo;
        private readonly AppDbContext _context;

        public CategoryService(IGenericRepository<Category> repo, AppDbContext context)
        {
            _repo    = repo;
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
            => await _context.Categories
                   .Include(c => c.Products)
                   .OrderBy(c => c.Name)
                   .ToListAsync();

        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories
                   .Include(c => c.Products)
                   .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<(bool success, string message)> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Назва категорії не може бути порожньою.");

            bool exists = await _context.Categories
                .AnyAsync(c => c.Name.ToLower() == name.Trim().ToLower());
            if (exists)
                return (false, "Категорія з такою назвою вже існує.");

            _context.Categories.Add(new Category { Name = name.Trim() });
            await _context.SaveChangesAsync();
            return (true, "Категорію створено.");
        }

        public async Task<(bool success, string message)> UpdateAsync(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Назва категорії не може бути порожньою.");

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return (false, "Категорію не знайдено.");

            bool duplicate = await _context.Categories
                .AnyAsync(c => c.Id != id && c.Name.ToLower() == name.Trim().ToLower());
            if (duplicate)
                return (false, "Категорія з такою назвою вже існує.");

            category.Name = name.Trim();
            await _context.SaveChangesAsync();
            return (true, "Категорію оновлено.");
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return (false, "Категорію не знайдено.");

            if (category.Products.Any())
                return (false, $"Неможливо видалити: в категорії є {category.Products.Count} товар(ів).");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return (true, "Категорію видалено.");
        }
    }
}

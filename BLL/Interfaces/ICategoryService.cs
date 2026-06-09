using Domain.Entities;

namespace BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<(bool success, string message)> CreateAsync(string name);
        Task<(bool success, string message)> UpdateAsync(int id, string name);
        Task<(bool success, string message)> DeleteAsync(int id);
    }
}

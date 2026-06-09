using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Interfaces
{
<<<<<<< HEAD
    internal interface IGenericRepository<T> where T : class
=======
    public interface IGenericRepository<T> where T : class
>>>>>>> Adminka
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T enitity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}

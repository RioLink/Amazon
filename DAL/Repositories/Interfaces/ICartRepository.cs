using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.Interfaces
{
    public interface ICartRepository : IGenericRepository<CartItem>
    {
        Task<int> CountByUserIdAsync(string userId);
    }
}

using DAL.Data;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class CartRepository : GenericRepository<CartItem>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<int> CountByUserIdAsync(string userId)
        {
            return await _context.CartItems.Where(c => c.UserId == userId).SumAsync(c => c.Quantity);
        }
    }
}

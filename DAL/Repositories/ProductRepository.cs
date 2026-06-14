using DAL.Data;
using DAL.Repositories.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories
{
    public class ProductRepository: GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public async Task<List<Product>> GetAllWithCategoriesAsync() 
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return products;
        }

        public async Task<Product?> GetByIdWithCategoryAsync(int id)
        {
            var product = await _context.Products.Where(p => p.Id == id).Include(p => p.Category).FirstOrDefaultAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> GetMostPopularProductsAsync()
        {
            var popularIds = await _context.Orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, Total = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(g => g.Total)
                .Take(4)
                .Select(g => g.ProductId)
                .ToListAsync();

            var result = new List<Product>();

            if (popularIds.Count > 0)
            {
                result = await _context.Products
                    .Where(p => popularIds.Contains(p.Id))
                    .Include(p => p.Category)
                    .ToListAsync();
            }

            if (result.Count < 4)
            {
                var existingIds = result.Select(p => p.Id).ToList();
                var extra = await _context.Products
                    .Where(p => !existingIds.Contains(p.Id))
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.Id)
                    .Take(4 - result.Count)
                    .ToListAsync();
                result.AddRange(extra);
            }

            return result;
        }
    }
}
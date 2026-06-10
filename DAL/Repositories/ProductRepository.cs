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
            // If orders exist — top products by order volume
            var popularIds = await _context.Orders
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, Total = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(g => g.Total)
                .Take(12)
                .Select(g => g.ProductId)
                .ToListAsync();

            if (popularIds.Count > 0)
            {
                return await _context.Products
                    .Where(p => popularIds.Contains(p.Id))
                    .Include(p => p.Category)
                    .ToListAsync();
            }

            // Fallback: newest products from DB
            return await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(12)
                .ToListAsync();
        }
    }
}
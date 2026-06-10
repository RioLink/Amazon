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

        public async
        Task<IEnumerable<Product>> GetMostPopularProductsAsync()
        {
            var products = await _context.Orders
                .Include( o => o.Items)
                .ThenInclude(oi => oi.Product)
                .SelectMany(o => o.Items)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(5)
                .Join(_context.Products, g => g.ProductId, p => p.Id, (g, p) => p)
                .Include(p => p.Category).ToListAsync();
            return products;
        }
    }
}
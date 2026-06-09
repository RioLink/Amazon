using DAL.Data;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeders
{
    public static class CategoriesSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Техніка" },
                    new Category { Name = "Одяг" },
                    new Category { Name = "Взуття" },
                    new Category { Name = "Книги" },
                    new Category { Name = "Іграшки" },
                    new Category { Name = "Спорт" },
                    new Category { Name = "Косметика" },
                    new Category { Name = "Меблі" },
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }
    }
}
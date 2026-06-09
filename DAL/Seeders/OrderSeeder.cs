using DAL.Data;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeders
{
    public static class OrderSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager)
        {
            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null) return;

            if (!await context.Orders.AnyAsync())
            {
                var product = await context.Products.FirstOrDefaultAsync();
                if (product == null) return;

                var order = new Order
                {
                    UserId = admin.Id,
                    Date = DateTime.UtcNow,
                    TotalAmount = product.Price,
                    Status = OrderStatus.Pending,
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            ProductId = product.Id,
                            Quantity = 1,
                            Price = product.Price
                        }
                    }
                };

                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }
        }
    }
}
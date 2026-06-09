using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace DAL.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(UserManager<User> userManager)
        {
            var admin = await userManager.FindByNameAsync("admin");

            if (admin == null)
            {
                var user = new User { UserName = "admin" };

                var result = await userManager.CreateAsync(user, "Admin123!");

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
            }
        }
    }
}
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Data.Context;
using Microsoft.AspNetCore.Identity;

namespace FoodDonationSystem.API.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await context.Database.EnsureCreatedAsync();

            await RoleSeeder.SeedAsync(roleManager);
        }
    }
}

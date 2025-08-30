using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Data.Context;
using Microsoft.AspNetCore.Identity;

namespace FoodDonationSystem.API.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider, ILogger logger)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await context.Database.EnsureCreatedAsync();

            await RoleSeeder.SeedAsync(roleManager, logger);
            await RoleSeeder.SeedAdminUser(userManager, logger);
        }
    }
}

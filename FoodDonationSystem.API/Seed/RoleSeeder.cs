using FoodDonationSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodDonationSystem.API.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new[]
             {
                new ApplicationRole("Admin") { Description = "System Administrator" },
                new ApplicationRole("Restaurant") { Description = "Restaurant Owner/Manager" },
                new ApplicationRole("Charity") { Description = "Charity Organization" },
                new ApplicationRole("Volunteer") { Description = "Volunteer Driver" },
                new ApplicationRole("Individual") { Description = "Individual Donor" }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(new ApplicationRole(role.Name));
                }
            }
        }
    }
}

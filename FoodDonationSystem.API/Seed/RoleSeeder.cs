using FoodDonationSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace FoodDonationSystem.API.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            var roles = new[]
            {
                new ApplicationRole("Admin") { Description = "مدير النظام - صلاحيات كاملة" },
                new ApplicationRole("Restaurant") { Description = "مطعم - يمكنه إضافة تبرعات طعام" },
                new ApplicationRole("Charity") { Description = "مؤسسة خيرية - يمكنها استقبال التبرعات" },
                new ApplicationRole("Volunteer") { Description = "متطوع - يمكنه توصيل التبرعات" },
                new ApplicationRole("Individual") { Description = "فرد - يمكنه التبرع بالطعام" }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!))
                {
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Created role: {RoleName}", role.Name);
                    }
                    else
                    {
                        logger.LogWarning("Failed to create role: {RoleName}. Errors: {Errors}",
                            role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Role already exists: {RoleName}", role.Name);
                }
            }
        }

        public static async Task SeedAdminUser(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            const string adminEmail = "admin@fooddonation.com";
            const string adminPassword = "Admin@123456";

            try
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Administrator",
                        IsVerified = true,
                        IsActive = true,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        logger.LogInformation("Admin user created successfully");
                    }
                    else
                    {
                        logger.LogWarning("Failed to create admin user. Errors: {Errors}",
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Admin user already exists");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating admin user");
            }
        }
    }
}

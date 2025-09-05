using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodDonationSystem.API.Seed
{
    public static class DataSeeder
    {



        public static async Task SeedAllData(IServiceProvider serviceProvider, ILogger logger, IWebHostEnvironment environment)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await context.Database.MigrateAsync();

            // Seed in order to respect foreign key constraints
            await SeedRoles(RoleManager, logger, context);

            if (environment.IsDevelopment())
            {
                await SeedUsers(userManager, logger, context);
                await RoleSeeder.SeedAdminUser(userManager, logger);

                await SeedRestaurants(context, logger, context);
                await SeedCharities(context, logger, context);
                await SeedVolunteers(context, logger, context);
                await SeedVolunteerAreas(context, logger, context);
                await SeedRestaurantSchedules(context, logger, context);
                await SeedDonations(context, logger, context);
                await SeedDonationImages(context, logger, context);
                await SeedCharityNeeds(context, logger, context);
                await SeedReservations(context, logger, context);
                await SeedDeliveries(context, logger, context);
                await SeedReviews(context, logger, context);
            }
            await RoleSeeder.SeedAdminUser(userManager, logger);

            await context.SaveChangesAsync();
            logger.LogInformation("All seeding data completed successfully!");
        }


        private static async Task SeedRoles(RoleManager<ApplicationRole> roleManager, ILogger logger, ApplicationDbContext context)
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
                        await context.SaveChangesAsync();

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
        private static async Task SeedUsers(UserManager<ApplicationUser> userManager, ILogger logger, ApplicationDbContext context)
        {
            if (await userManager.Users.AnyAsync())
            {
                logger.LogInformation("Users already exist, skipping user seeding.");
                return;
            }


            var users = new List<ApplicationUser>
            {
                // Admin User
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin2@fooddonation.com",
                    Email = "admin2@fooddonation.com",
                    FirstName = "أحمد",
                    LastName = "ضياء",
                    IsVerified = true,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567890"
                },
                // Restaurant Users
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "restaurant1@example.com",
                    Email = "restaurant1@example.com",
                    FirstName = "محمد",
                    LastName = "ضياء",
                    IsVerified = true,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567891"
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "restaurant2@example.com",
                    Email = "restaurant2@example.com",
                    FirstName = "فاطمة",
                    LastName = "السيد",
                    IsVerified = true,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567892"
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "restaurant3@example.com",
                    Email = "restaurant3@example.com",
                    FirstName = "علي",
                    LastName = "ماهر",
                    IsVerified = false,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567893"
                },
                // Charity Users
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "charity1@example.com",
                    Email = "charity1@example.com",
                    FirstName = "سارة",
                    LastName = "هلال",
                    IsVerified = true,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567894"
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "charity2@example.com",
                    Email = "charity2@example.com",
                    FirstName = "خالد",
                    LastName = "مؤمن",
                    IsVerified = true,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567895"
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "charity3@example.com",
                    Email = "charity3@example.com",
                    FirstName = "نورا",
                    LastName = "علي",
                    IsVerified = false,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567896"
                },
                // Volunteer Users
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "volunteer1@example.com",
                    Email = "volunteer1@example.com",
                    FirstName = "عبدالله",
                    LastName = "محمود",
                    IsVerified = true,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567897"
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "volunteer2@example.com",
                    Email = "volunteer2@example.com",
                    FirstName = "مريم",
                    LastName = "بلال",
                    IsVerified = true,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567898"
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "volunteer3@example.com",
                    Email = "volunteer3@example.com",
                    FirstName = "مروه",
                    LastName = "عبدالله",
                    IsVerified = false,
                    IsActive = true,
                    EmailConfirmed = true,
                    PhoneNumber = "+201234567899"
                }
            };

            foreach (var user in users)
            {
                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    // Assign appropriate role based on email
                    string roleToAssign = user.Email!.Contains("admin") ? "Admin" :
                                        user.Email.Contains("restaurant") ? "Restaurant" :
                                        user.Email.Contains("charity") ? "Charity" :
                                        user.Email.Contains("volunteer") ? "Volunteer" : "Individual";

                    var roleResult = await userManager.AddToRoleAsync(user, roleToAssign);
                    if (roleResult.Succeeded)
                    {
                        await context.SaveChangesAsync();

                        logger.LogInformation($"User {user.UserName} created successfully with role {roleToAssign}.");
                    }
                    else
                    {
                        logger.LogError($"Failed to assign role {roleToAssign} to user {user.UserName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogError($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

        }

        private static async Task SeedRestaurants(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.Restaurants.AnyAsync())
            {
                logger.LogInformation("Restaurants already exist, skipping restaurant seeding.");
                return;
            }

            var restaurantUsers = await context.Users
                .Where(u => u.Email!.Contains("restaurant"))
                .ToListAsync();

            if (restaurantUsers.Count < 3)
            {
                logger.LogError($"Expected 3 restaurant users, but found {restaurantUsers.Count}. Skipping restaurant seeding.");
                return;
            }

            var restaurants = new List<Restaurant>
            {
                new Restaurant
                {
                    Name = "مطعم الأصالة",
                    Description = "مطعم يقدم الأطباق المصرية الأصيلة",
                    Address = "شارع التحرير، القاهرة",
                    Latitude = 30.0444,
                    Longitude = 31.2357,
                    LicenseDocument = "license_doc_1.pdf",
                    CommercialRegister = "CR123456",
                    Status = ApprovalStatus.Approved,
                    IsActive = true,
                    UserId = restaurantUsers[0].Id
                },
                new Restaurant
                {
                    Name = "مطعم الشام",
                    Description = "أشهى المأكولات الشامية",
                    Address = "شارع الهرم، الجيزة",
                    Latitude = 30.0131,
                    Longitude = 31.2089,
                    LicenseDocument = "license_doc_2.pdf",
                    CommercialRegister = "CR123457",
                    Status = ApprovalStatus.Approved,
                    IsActive = true,
                    UserId = restaurantUsers[1].Id
                },
                new Restaurant
                {
                    Name = "مطعم البحر الأحمر",
                    Description = "أفضل المأكولات البحرية",
                    Address = "كورنيش الإسكندرية",
                    Latitude = 31.2001,
                    Longitude = 29.9187,
                    LicenseDocument = "license_doc_3.pdf",
                    CommercialRegister = "CR123458",
                    Status = ApprovalStatus.Pending,
                    IsActive = true,
                    UserId = restaurantUsers[2].Id
                }
            };

            context.Restaurants.AddRange(restaurants);
            await context.SaveChangesAsync();
            logger.LogInformation("Restaurants seeded successfully.");

        }

        private static async Task SeedCharities(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.Charities.AnyAsync())
            {
                logger.LogInformation("Charities already exist, skipping charity seeding.");
                return;
            }

            var charityUsers = await context.Users
                .Where(u => u.Email!.Contains("charity"))
                .ToListAsync();

            if (charityUsers.Count < 3)
            {
                logger.LogError($"Expected 3 charity users, but found {charityUsers.Count}. Skipping charity seeding.");
                return;
            }

            var charities = new List<Charity>
            {
                new Charity
                {
                    Name = "دار الأيتام الخيرية",
                    Description = "رعاية الأيتام وتقديم المساعدة لهم",
                    Address = "حي المعادي، القاهرة",
                    Latitude = 29.9602,
                    Longitude = 31.2578,
                    Capacity = 100,
                    LicenseDocument = "charity_license_1.pdf",
                    ProofDocument = "charity_proof_1.pdf",
                    Status = ApprovalStatus.Approved,
                    IsActive = true,
                    Type = CharityType.Orphanage,
                    UserId = charityUsers[0].Id
                },
                new Charity
                {
                    Name = "دار المسنين الرحمة",
                    Description = "رعاية كبار السن وتقديم الخدمات لهم",
                    Address = "حي الزمالك، القاهرة",
                    Latitude = 30.0626,
                    Longitude = 31.2197,
                    Capacity = 50,
                    LicenseDocument = "charity_license_2.pdf",
                    ProofDocument = "charity_proof_2.pdf",
                    Status = ApprovalStatus.Approved,
                    IsActive = true,
                    Type = CharityType.ElderlyHome,
                    UserId = charityUsers[1].Id
                },
                new Charity
                {
                    Name = "مأوى المشردين",
                    Description = "إيواء المشردين وتقديم المساعدة",
                    Address = "حي العجوزة، الجيزة",
                    Latitude = 30.0444,
                    Longitude = 31.2000,
                    Capacity = 75,
                    LicenseDocument = "charity_license_3.pdf",
                    ProofDocument = "charity_proof_3.pdf",
                    Status = ApprovalStatus.Pending,
                    IsActive = true,
                    Type = CharityType.Shelter,
                    UserId = charityUsers[2].Id
                }
            };

            context.Charities.AddRange(charities);
            await context.SaveChangesAsync();
            logger.LogInformation("Charities seeded successfully.");

        }

        private static async Task SeedVolunteers(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.Volunteers.AnyAsync())
            {
                logger.LogInformation("Volunteers already exist, skipping volunteer seeding.");
                return;
            }

            var volunteerUsers = await context.Users
                .Where(u => u.Email!.Contains("volunteer"))
                .ToListAsync();

            if (volunteerUsers.Count < 3)
            {
                logger.LogError($"Expected 3 volunteer users, but found {volunteerUsers.Count}. Skipping volunteer seeding.");
                return;
            }

            var volunteers = new List<Volunteer>
            {
                new Volunteer
                {
                    VehicleType = "سيارة صغيرة",
                    VehicleNumber = "ABC-1234",
                    DriverLicense = "DL123456",
                    IsActive = true,
                    UserId = volunteerUsers[0].Id
                },
                new Volunteer
                {
                    VehicleType = "فان",
                    VehicleNumber = "DEF-5678",
                    DriverLicense = "DL123457",
                    IsActive = true,
                    UserId = volunteerUsers[1].Id
                },
                new Volunteer
                {
                    VehicleType = "شاحنة صغيرة",
                    VehicleNumber = "GHI-9012",
                    DriverLicense = "DL123458",
                    IsActive = false,
                    UserId = volunteerUsers[2].Id
                }
            };

            context.Volunteers.AddRange(volunteers);
            await context.SaveChangesAsync();
            logger.LogInformation("Volunteers seeded successfully.");

        }

        private static async Task SeedVolunteerAreas(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.VolunteerAreas.AnyAsync())
            {
                logger.LogInformation("Volunteer areas already exist, skipping volunteer area seeding.");
                return;
            }

            var volunteers = await context.Volunteers.ToListAsync();

            if (volunteers.Count < 3)
            {
                logger.LogError($"Expected 3 volunteers, but found {volunteers.Count}. Skipping volunteer area seeding.");
                return;
            }

            var volunteerAreas = new List<VolunteerArea>
            {
                new VolunteerArea
                {
                    AreaName = "القاهرة - وسط المدينة",
                    CenterLatitude = 30.0444,
                    CenterLongitude = 31.2357,
                    RadiusKm = 10,
                    VolunteerId = volunteers[0].Id
                },
                new VolunteerArea
                {
                    AreaName = "القاهرة - المعادي",
                    CenterLatitude = 29.9602,
                    CenterLongitude = 31.2578,
                    RadiusKm = 15,
                    VolunteerId = volunteers[0].Id
                },
                new VolunteerArea
                {
                    AreaName = "الجيزة - الهرم",
                    CenterLatitude = 30.0131,
                    CenterLongitude = 31.2089,
                    RadiusKm = 12,
                    VolunteerId = volunteers[1].Id
                },
                new VolunteerArea
                {
                    AreaName = "الإسكندرية - الكورنيش",
                    CenterLatitude = 31.2001,
                    CenterLongitude = 29.9187,
                    RadiusKm = 8,
                    VolunteerId = volunteers[2].Id
                }
            };

            context.VolunteerAreas.AddRange(volunteerAreas);
            await context.SaveChangesAsync();
            logger.LogInformation("Volunteer areas seeded successfully.");

        }

        private static async Task SeedRestaurantSchedules(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.RestaurantSchedules.AnyAsync())
            {
                logger.LogInformation("Restaurant schedules already exist, skipping restaurant schedule seeding.");
                return;
            }

            var restaurants = await context.Restaurants.ToListAsync();

            if (restaurants.Count == 0)
            {
                logger.LogError("No restaurants found. Skipping restaurant schedule seeding.");
                return;
            }

            var schedules = new List<RestaurantSchedule>();

            foreach (var restaurant in restaurants)
            {
                // Add schedules for each day of the week
                for (int day = 0; day < 7; day++)
                {
                    schedules.Add(new RestaurantSchedule
                    {
                        DayOfWeek = (DayOfWeek)day,
                        DonationTime = new TimeSpan(18, 0, 0), // 6:00 PM
                        IsActive = true,
                        RestaurantId = restaurant.Id
                    });
                }
            }

            context.RestaurantSchedules.AddRange(schedules);
            await context.SaveChangesAsync();
            logger.LogInformation("Restaurant schedules seeded successfully.");

        }

        private static async Task SeedDonations(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.Donations.AnyAsync())
            {
                logger.LogInformation("Donations already exist, skipping donation seeding.");
                return;
            }

            var restaurants = await context.Restaurants.ToListAsync();

            if (restaurants.Count == 0)
            {
                logger.LogError("No restaurants found. Skipping donation seeding.");
                return;
            }

            var donations = new List<Donation>
            {
                new Donation
                {
                    FoodType = "أرز بالدجاج",
                    Description = "أرز مطبوخ مع قطع الدجاج والخضار",
                    EstimatedServings = 50,
                    ExpiryDateTime = DateTime.UtcNow.AddHours(4),
                    Status = DonationStatus.Available,
                    RequiresPickup = true,
                    SpecialInstructions = "يجب التسليم قبل الساعة 8 مساءً",
                    ContactPerson = "محمد الطاهي",
                    ContactPhone = "+201234567891",
                    RestaurantId = restaurants[0].Id
                },
                new Donation
                {
                    FoodType = "كبسة لحم",
                    Description = "كبسة لحم مع الأرز البسمتي",
                    EstimatedServings = 30,
                    ExpiryDateTime = DateTime.UtcNow.AddHours(6),
                    Status = DonationStatus.Available,
                    RequiresPickup = true,
                    SpecialInstructions = "جاهز للتسليم",
                    ContactPerson = "فاطمة المطعم",
                    ContactPhone = "+201234567892",
                    RestaurantId = restaurants[1].Id
                },
                new Donation
                {
                    FoodType = "سمك مشوي",
                    Description = "سمك مشوي مع الأرز والخضار",
                    EstimatedServings = 25,
                    ExpiryDateTime = DateTime.UtcNow.AddHours(3),
                    Status = DonationStatus.Reserved,
                    RequiresPickup = true,
                    SpecialInstructions = "محجوز للجمعية الخيرية",
                    ContactPerson = "علي الطباخ",
                    ContactPhone = "+201234567893",
                    RestaurantId = restaurants[2].Id
                },
                new Donation
                {
                    FoodType = "شوربة خضار",
                    Description = "شوربة خضار طازجة",
                    EstimatedServings = 40,
                    ExpiryDateTime = DateTime.UtcNow.AddHours(5),
                    Status = DonationStatus.Available,
                    RequiresPickup = false,
                    SpecialInstructions = "يمكن التسليم في الموقع",
                    ContactPerson = "محمد الطاهي",
                    ContactPhone = "+201234567891",
                    RestaurantId = restaurants[0].Id
                }
            };

            context.Donations.AddRange(donations);
            await context.SaveChangesAsync();
            logger.LogInformation("Donations seeded successfully.");

        }

        private static async Task SeedDonationImages(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.DonationImages.AnyAsync())
            {
                logger.LogInformation("Donation images already exist, skipping donation image seeding.");
                return;
            }

            var donations = await context.Donations.ToListAsync();

            if (donations.Count == 0)
            {
                logger.LogError("No donations found. Skipping donation image seeding.");
                return;
            }

            var donationImages = new List<DonationImage>
            {
                new DonationImage
                {
                    ImagePath = "/uploads/donations/rice_chicken_1.jpg",
                    IsPrimary = true,
                    DonationId = donations[0].Id
                },
                new DonationImage
                {
                    ImagePath = "/uploads/donations/rice_chicken_2.jpg",
                    IsPrimary = false,
                    DonationId = donations[0].Id
                },
                new DonationImage
                {
                    ImagePath = "/uploads/donations/kabsa_1.jpg",
                    IsPrimary = true,
                    DonationId = donations[1].Id
                },
                new DonationImage
                {
                    ImagePath = "/uploads/donations/fish_1.jpg",
                    IsPrimary = true,
                    DonationId = donations[2].Id
                },
                new DonationImage
                {
                    ImagePath = "/uploads/donations/soup_1.jpg",
                    IsPrimary = true,
                    DonationId = donations[3].Id
                }
            };

            context.DonationImages.AddRange(donationImages);
            await context.SaveChangesAsync();
            logger.LogInformation("Donation images seeded successfully.");

        }

        private static async Task SeedCharityNeeds(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.CharityNeeds.AnyAsync())
            {
                logger.LogInformation("Charity needs already exist, skipping charity need seeding.");
                return;
            }

            var charities = await context.Charities.ToListAsync();

            if (charities.Count == 0)
            {
                logger.LogError("No charities found. Skipping charity need seeding.");
                return;
            }

            var charityNeeds = new List<CharityNeed>
            {
                new CharityNeed
                {
                    FoodType = "رز وفراخ",
                    RequiredServings = 30,
                    Description = "نحتاج طعام ساخن للأطفال",
                    ValidUntil = DateTime.UtcNow.AddDays(1),
                    CharityId = charities[0].Id
                },
                new CharityNeed
                {
                    FoodType = "حلويات",
                    RequiredServings = 20,
                    Description = "حلويات شرقيه",
                    ValidUntil = DateTime.UtcNow.AddDays(2),
                    CharityId = charities[1].Id
                },
                new CharityNeed
                {
                    FoodType = "لحوم",
                    RequiredServings = 25,
                    Description = "لحمه اي حاجه بقا",
                    ValidUntil = DateTime.UtcNow.AddDays(1),
                    CharityId = charities[2].Id
                }
            };

            context.CharityNeeds.AddRange(charityNeeds);
            await context.SaveChangesAsync();
            logger.LogInformation("Charity needs seeded successfully.");

        }

        private static async Task SeedReservations(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.Reservations.AnyAsync())
            {
                logger.LogInformation("Reservations already exist, skipping reservation seeding.");
                return;
            }

            var donations = await context.Donations.ToListAsync();
            var charities = await context.Charities.ToListAsync();
            var volunteers = await context.Volunteers.ToListAsync();

            if (donations.Count == 0 || charities.Count == 0 || volunteers.Count == 0)
            {
                logger.LogError($"Missing required data for reservations. Donations: {donations.Count}, Charities: {charities.Count}, Volunteers: {volunteers.Count}. Skipping reservation seeding.");
                return;
            }

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    ReservationTime = DateTime.UtcNow.AddHours(2),
                    Status = ReservationStatus.Confirmed,
                    Notes = "سيتم الاستلام في الوقت المحدد",
                    PickupTime = DateTime.UtcNow.AddHours(2),
                    PickupPersonName = "سارة الجمعية",
                    PickupPersonPhone = "+201234567894",
                    VolunteerId = volunteers[0].Id,
                    DonationId = donations[2].Id, // السمك المشوي المحجوز
                    CharityId = charities[0].Id
                },
                new Reservation
                {
                    ReservationTime = DateTime.UtcNow.AddHours(1),
                    Status = ReservationStatus.Pending,
                    Notes = "في انتظار التأكيد",
                    DonationId = donations[0].Id, // الأرز بالدجاج
                    CharityId = charities[1].Id
                }
            };

            context.Reservations.AddRange(reservations);
            await context.SaveChangesAsync();
            logger.LogInformation("Reservations seeded successfully.");

        }

        private static async Task SeedDeliveries(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.Deliveries.AnyAsync())
            {
                logger.LogInformation("Deliveries already exist, skipping delivery seeding.");
                return;
            }

            var reservations = await context.Reservations.ToListAsync();
            var volunteers = await context.Volunteers.ToListAsync();

            if (reservations.Count == 0 || volunteers.Count == 0)
            {
                logger.LogError($"Missing required data for deliveries. Reservations: {reservations.Count}, Volunteers: {volunteers.Count}. Skipping delivery seeding.");
                return;
            }

            var deliveries = new List<Delivery>
            {
                new Delivery
                {
                    PickupTime = DateTime.UtcNow.AddHours(1),
                    DeliveryTime = DateTime.UtcNow.AddHours(2),
                    Status = DeliveryStatus.Delivered,
                    DeliveryNotes = "تم التسليم بنجاح",
                    ProofOfDelivery = "/uploads/proofs/delivery_1.jpg",
                    ReservationId = reservations[0].Id,
                    VolunteerId = volunteers[0].Id
                }
            };

            context.Deliveries.AddRange(deliveries);
            await context.SaveChangesAsync();
            logger.LogInformation("Deliveries seeded successfully.");

        }

        private static async Task SeedReviews(ApplicationDbContext context, ILogger logger, ApplicationDbContext context1)
        {
            if (await context.Reviews.AnyAsync())
            {
                logger.LogInformation("Reviews already exist, skipping review seeding.");
                return;
            }

            var users = await context.Users.ToListAsync();
            var reservations = await context.Reservations.ToListAsync();

            if (users.Count == 0 || reservations.Count == 0)
            {
                logger.LogError($"Missing required data for reviews. Users: {users.Count}, Reservations: {reservations.Count}. Skipping review seeding.");
                return;
            }

            var reviews = new List<Review>
            {
                new Review
                {
                    Rating = 5,
                    Comment = "طعام ممتاز وخدمة رائعة",
                    Type = ReviewType.CharityToRestaurant,
                    FromUserId = users.First(u => u.Email!.Contains("charity")).Id,
                    ToUserId = users.First(u => u.Email!.Contains("restaurant")).Id,
                    ReservationId = reservations[0].Id
                },
                new Review
                {
                    Rating = 4,
                    Comment = "متطوع محترف ومتعاون",
                    Type = ReviewType.VolunteerRating,
                    FromUserId = users.First(u => u.Email!.Contains("charity")).Id,
                    ToUserId = users.First(u => u.Email!.Contains("volunteer")).Id,
                    ReservationId = reservations[0].Id
                }
            };

            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();
            logger.LogInformation("Reviews seeded successfully.");

        }
    }
}

using EasyGames.Data;
using EasyGames.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Services
{
    // SeedService sets up the database with roles, an admin user, categories, and demo stock items
    public class SeedService
    {
        // Seeds the database with initial data
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {            
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                // Ensure database is created and apply migrations automatically
                logger.LogInformation("Applying migrations");
                await context.Database.MigrateAsync();

                // Seed Roles (Admin, User)
                logger.LogInformation("Seeding roles");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");

                // Admin user
                logger.LogInformation("Seeding admin user");
                var adminEmail = "admin@easygames.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                // Create admin user if it doesn't exist
                if (adminUser == null)
                {
                    adminUser = new Users
                    {
                        FullName = "Site Admin",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    // Create the user with a default password
                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (!result.Succeeded)
                    {
                        logger.LogError("Failed to create admin user: {Errors}",
                            string.Join(",", result.Errors.Select(e => e.Description)));
                        return;
                    }
                }

                // Ensure admin user has the Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    logger.LogInformation("Assigning Admin role");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Seed Categories (Books, Games, Toys)
                var mustHave = new[] { "Books", "Games", "Toys" };
                var existingNames = await context.Categories.Select(c => c.Name).ToListAsync();

                // Find missing categories and add them
                var toAddCats = mustHave
                    .Except(existingNames, StringComparer.OrdinalIgnoreCase)
                    .Select(n => new Category { Name = n })
                    .ToList();

                // Add missing categories to the database
                if (toAddCats.Any())
                {
                    logger.LogInformation("Adding missing categories: {Cats}", string.Join(", ", toAddCats.Select(c => c.Name)));
                    context.Categories.AddRange(toAddCats);
                    await context.SaveChangesAsync();
                }

                // Load categories into a dictionary for easy access
                var cat = await context.Categories
                    .ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);

                // Seed demo StockItems if none exist
                if (!await context.StockItems.AnyAsync())
                {
                    logger.LogInformation("Seeding demo stock items");

                    context.StockItems.AddRange(
                        new StockItem
                        {
                            Title = "Clean Code",
                            Description = "Robert C. Martin",
                            Price = 39.99m,
                            Quantity = 12,
                            CategoryId = cat["Books"].Id,
                            ImageUrl = "https://picsum.photos/seed/cleancode/400/600"
                        },
                        new StockItem
                        {
                            Title = "1984",
                            Description = "George Orwell",
                            Price = 15.50m,
                            Quantity = 25,
                            CategoryId = cat["Books"].Id,
                            ImageUrl = "https://picsum.photos/seed/1984/400/600"
                        },
                        new StockItem
                        {
                            Title = "Legend of Zelda",
                            Description = "Nintendo Switch",
                            Price = 79.00m,
                            Quantity = 6,
                            CategoryId = cat["Games"].Id,
                            ImageUrl = "https://picsum.photos/seed/zelda/400/400"
                        },
                        new StockItem
                        {
                            Title = "Lego Classic Box",
                            Description = "Creative bricks",
                            Price = 49.00m,
                            Quantity = 8,
                            CategoryId = cat["Toys"].Id,
                            ImageUrl = "https://picsum.photos/seed/lego/400/400"
                        }
                    );

                    // Save the new stock items to the database
                    await context.SaveChangesAsync();
                }
            }

            // Log any errors that occur during the seeding process
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        // Adds a role if it doesn't already exist
        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}

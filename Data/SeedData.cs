using EasyGames.Models;
using Microsoft.AspNetCore.Identity;

namespace EasyGames.Data
{
    // SeedData class is used here to initialize roles and a default admin user
    public static class SeedData
    {
        // SeedAsync method is used here to create roles and a default admin user
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>(); // Role manager service
            var userMgr = services.GetRequiredService<UserManager<Users>>(); // User manager service

            // Create roles if missing
            foreach (var r in new[] { "Admin", "User" })
            {
                if (!await roleMgr.RoleExistsAsync(r))
                {
                    var rr = await roleMgr.CreateAsync(new IdentityRole(r));
                    if (!rr.Succeeded)
                        throw new Exception("Role create failed: " + string.Join("; ", rr.Errors.Select(e => e.Description)));
                }
            }

            // Create admin user if missing
            var email = "admin@easygames.local";
            var admin = await userMgr.FindByEmailAsync(email);
            if (admin == null)
            {
                admin = new Users
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "Site Admin"
                };

                // Create the user with a default password
                var ur = await userMgr.CreateAsync(admin, "Admin@123");
                if (!ur.Succeeded)
                    throw new Exception("User create failed: " + string.Join("; ", ur.Errors.Select(e => e.Description)));
            }

            // Ensure admin user has the Admin role
            if (!await userMgr.IsInRoleAsync(admin, "Admin"))
            {
                // Assign Admin role to the user
                var ar = await userMgr.AddToRoleAsync(admin, "Admin");
                if (!ar.Succeeded)
                    throw new Exception("AddToRole failed: " + string.Join("; ", ar.Errors.Select(e => e.Description)));
            }


        }
    }
}

using Microsoft.AspNetCore.Identity;
using GestionPrestation.Models; // Reference to your custom ApplicationUser
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace GestionPrestation.Utility
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Get necessary services from the service provider
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // --- 1. Define and Create Roles ---
            string[] roleNames = { "Admin", "Manager", "Client" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // --- 2. Create Default Admin User ---
            var defaultAdminEmail = "admin@prestation.com";
            var defaultAdminPassword = "AdminP@ssword123!";

            // Check if the admin user already exists
            var user = await userManager.FindByEmailAsync(defaultAdminEmail);

            if (user == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = defaultAdminEmail,
                    Email = defaultAdminEmail,
                    EmailConfirmed = true,
                    // Supply required custom fields
                    FirstName = "System",
                    LastName = "Admin"
                };

                // Create the user and assign password
                var result = await userManager.CreateAsync(adminUser, defaultAdminPassword);

                if (result.Succeeded)
                {
                    // Find the user object again (or use the original if not detached) and assign the Admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
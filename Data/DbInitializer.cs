using System;
using System.Linq;
using System.Threading.Tasks;
using GestionPrestation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GestionPrestation.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Roles
            string[] roles = { "Admin", "Prestataire", "Client", "Societe" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var r = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!r.Succeeded)
                    {
                        var errors = string.Join("; ", r.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to create role '{role}': {errors}");
                    }
                }
            }

            // Admin
            string adminEmail = "admin@admin.com";
            string adminPassword = "Admin@123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    // Champs requis par votre modèle ApplicationUser
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    var addRoleResult = await userManager.AddToRoleAsync(admin, "Admin");
                    if (!addRoleResult.Succeeded)
                    {
                        var errors = string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Failed to add Admin role to seeded user: {errors}");
                    }
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Seed admin creation failed: {errors}");
                }
            }
        }
    }
}

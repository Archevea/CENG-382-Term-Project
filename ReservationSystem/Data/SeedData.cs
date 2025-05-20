// Generative AI technologies were used in the preparation of this page.
using Microsoft.AspNetCore.Identity;
using ReservationSystem.Models;

namespace ReservationSystem.Data
{
    // This class is responsible for seeding initial data into the database.
    // It creates roles and an admin user if they do not already exist.
    public static class SeedData
    {
        // Initializes the database with default roles and an admin user
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Get the RoleManager and UserManager services
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Define the roles to be created
            string[] roles = { "Admin", "Instructor" };

            // Create each role if it does not exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        
            // Define the default admin user email
            const string adminEmail = "admin@example.com";
            // Check if the admin user already exists
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                // Create the admin user
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    IsActive = true
                };

                // Create the admin user with a default password
                var result = await userManager.CreateAsync(adminUser, "AdminPass1!");
                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}

//using LibrarySystem.Repository.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Threading.Tasks;

//namespace DlmsWebApi.Helpers
//{
//    /// <summary>
//    /// Static utility class responsible for seeding the SQLite database with 
//    /// default security roles and users on application start.
//    /// </summary>
//    public static class SeedData
//    {
//        public static async Task SeedIdentityAsync(IServiceProvider services)
//        {
//            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

//            // 1. Seed Roles
//            string[] roles = { "SuperAdmin", "Staff" };
//            foreach (var role in roles)
//            {
//                if (!await roleManager.RoleExistsAsync(role))
//                {
//                    await roleManager.CreateAsync(new IdentityRole(role));
//                }
//            }

//            // 2. Seed default SuperAdmin User
//            var adminUser = await userManager.FindByNameAsync("admin");
//            if (adminUser == null)
//            {
//                adminUser = new ApplicationUser
//                {
//                    UserName = "admin",
//                    Email = "admin@library.com",
//                    EmailConfirmed = true,
//                    FullName = "System Administrator",
//                    CustomNotes = "Root administrator seeded during setup."
//                };

//                // The standard CreateAsync hashes the password securely using PBKDF2
//                var result = await userManager.CreateAsync(adminUser, "adminpassword");
//                if (result.Succeeded)
//                {
//                    await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
//                }
//            }

//            // 3. Seed default Staff User
//            var staffUser = await userManager.FindByNameAsync("staff");
//            if (staffUser == null)
//            {
//                staffUser = new ApplicationUser
//                {
//                    UserName = "staff",
//                    Email = "staff@library.com",
//                    EmailConfirmed = true,
//                    FullName = "Staff Operator",
//                    CustomNotes = "Staff member seeded during setup."
//                };

//                var result = await userManager.CreateAsync(staffUser, "staffpassword");
//                if (result.Succeeded)
//                {
//                    await userManager.AddToRoleAsync(staffUser, "Staff");
//                }
//            }
//        }
//    }
//}

using FinTracker.Models.Database;
using FinTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace FinTracker.Data
{
    public class DbInitializer
    {
        public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<FintrackContext>();

            string[] roles = { "Accountant", "Employee" };

            foreach (var role in roles)
            {
                var roleExist = await roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            string adminLogin = "admin", adminPassword = "admin";
            var adminUser = await userManager.FindByNameAsync(adminLogin);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = adminLogin, EmployeeId = null };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Accountant");
                }
            }
            
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { CategoryName = "Выплата ЗП" },
                    new Category { CategoryName = "Страховые взносы" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}

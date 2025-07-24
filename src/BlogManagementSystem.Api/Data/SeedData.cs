using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogManagementSystem.Api.Models;

namespace BlogManagementSystem.Api.Data
{
    public static class SeedData
    {
        private const string AdminRole = "Admin";
        private const string AuthorRole = "Author";
        private const string AdminUsername = "admin";
        private const string AdminEmail = "admin@blog.com";
        private const string AdminPassword = "Admin@123";

        public static void Seed(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role-admin", Name = AdminRole, NormalizedName = AdminRole.ToUpper() },
                new IdentityRole { Id = "role-author", Name = AuthorRole, NormalizedName = AuthorRole.ToUpper() }
            );
        }

        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            if (!await roleManager.RoleExistsAsync(AdminRole))
                await roleManager.CreateAsync(new IdentityRole(AdminRole));
            if (!await roleManager.RoleExistsAsync(AuthorRole))
                await roleManager.CreateAsync(new IdentityRole(AuthorRole));

            if (await userManager.FindByNameAsync(AdminUsername) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName    = AdminUsername,
                    Email       = AdminEmail,
                    FirstName   = "System",
                    LastName    = "Administrator",
                };
                await userManager.CreateAsync(admin, AdminPassword);
                await userManager.AddToRoleAsync(admin, AdminRole);
            }
        }
    }
}

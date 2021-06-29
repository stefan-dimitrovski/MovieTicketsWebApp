using Microsoft.AspNetCore.Identity;
using MovieShop.Domain.DomainModels;
using MovieShop.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieShop.Repository
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<MovieShopApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Standard.ToString()));
        }

        public static async Task SeedSuperAdminAsync(UserManager<MovieShopApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new MovieShopApplicationUser
            {
                UserName = "admin@admin.com",
                NormalizedUserName = "admin@admin.com",
                Email = "admin@admin.com",
                FirstName = "Admin",
                LastName = "Stefan",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserCart = new ShoppingCart()
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Test123!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }

            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserService.Infrastructure.Identity.Models;

namespace UserService.Infrastructure.Identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                Name = "admin",
                PhoneNumber = "79999999999",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (!await userManager.Users.AnyAsync())
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "admin");
                }
            }
        }
    }
}

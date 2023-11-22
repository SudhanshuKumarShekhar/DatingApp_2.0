using DatingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Data
{
    public class Seed
    {
        public static async Task SeedUser(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // create role 
            var roles = new List<AppRole>
            {
                new AppRole{Name = "Member"},
                new AppRole{Name = "Admin"},
                new AppRole{Name = "Moderator"}
            };
            // seed role in data
            foreach(var role in roles)
            {
                roleManager.CreateAsync(role);
            }

            // create admin
            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Admin@1");
            await userManager.AddToRolesAsync(admin, new[] {"Admin","Moderator"});
        }
    }
}

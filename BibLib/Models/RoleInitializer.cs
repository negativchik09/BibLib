using System.Threading.Tasks;
using BibLib.Service;
using Microsoft.AspNetCore.Identity;

namespace BibLib.Models
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = Config.AdminEmail;
            string password = Config.AdminPassword;
            if (await roleManager.FindByNameAsync(Config.AdminRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(Config.AdminRole));
            }
            if (await roleManager.FindByNameAsync(Config.LibrarianRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(Config.LibrarianRole));
            }
            if (await roleManager.FindByNameAsync(Config.PremiumRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(Config.PremiumRole));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                IdentityUser admin = new IdentityUser{ Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(admin, new []{Config.AdminRole, Config.LibrarianRole, Config.PremiumRole});
                }
            }
        }
    }
}
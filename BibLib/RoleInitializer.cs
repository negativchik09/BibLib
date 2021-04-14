using System.Threading.Tasks;
using BibLib.Service;
using Microsoft.AspNetCore.Identity;

namespace BibLib
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string adminEmail = Config.AdminEmail;
            string password = Config.AdminPassword;
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("premium") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("premium"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                IdentityUser admin = new IdentityUser{ Email = adminEmail, UserName = adminEmail };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(admin, new []{"admin", "premium"});
                }
            }
        }
    }
}
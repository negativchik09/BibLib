using System.Collections.Generic;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BibLib.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Registration(string returnUrl)
        {
            return View();
        }
        /*
[HttpPost]
public async Task<IActionResult> Registration(RegistrationViewModel model, string returnUrl)
{

 if (ModelState.IsValid)
{
    IdentityUser user = await _userManager.FindByEmailAsync(model.Email);
    if (user == default)
    {
        user = new IdentityUser
        {
            Email = model.Email,
            UserName = model.Email,
            PhoneNumber = model.Phone,
            PhoneNumberConfirmed = true,
            // Email confirmation don`t work, because i haven`t money for rent a smtp server.
            // If smtp will work switch this bool to false
            EmailConfirmed = true
        };
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            await _userManager.AddToRoleAsync(user, "user");
            await _ctx.Carts.AddAsync(new CartModel
            {
                User = user,
                CartItems = new List<CartItemModel>(),
            });
            await _ctx.SaveChangesAsync();
            return View("CheckEmail", returnUrl ?? "/");
        }
    }
    ModelState.AddModelError(nameof(RegistrationViewModel.Email), "Пользователь с такой почтой уже существует");
}
return View("Registration", model);

}
*/
    }
}
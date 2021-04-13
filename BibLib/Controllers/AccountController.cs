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
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                IdentityUser identityUser = await _userManager.FindByEmailAsync(model.Email);
                if (identityUser == default) 
                { 
                    identityUser = new IdentityUser
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        EmailConfirmed = true
                    };
                    IdentityResult result = await _userManager.CreateAsync(identityUser, model.Password);
                    if (result.Succeeded)
                    {
                        IdentityRole admin = new IdentityRole();
                        IdentityRole premium = new IdentityRole();
                        IdentityRole user = new IdentityRole();
                        _roleManager.CreateAsync(admin);
                        _roleManager.CreateAsync(premium);
                        _roleManager.CreateAsync(user);
                        await _signInManager.SignInAsync(identityUser, false);
                        await _userManager.AddToRoleAsync(identityUser, "user");
                    }
                }
                ModelState.AddModelError(nameof(RegistrationViewModel.Email), "Пользователь с такой почтой уже существует");
            }
            return View("Registration", model);
        }

        public IActionResult Login(string returnUrl)
        {
            return null;
        }
    }
}
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using BibLib.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BibLib.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _ctx;
        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, AppDbContext ctx)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _ctx = ctx;
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
                        await _signInManager.SignInAsync(identityUser, true);
                        await _userManager.AddToRoleAsync(identityUser, "user");
                        await _ctx.SecretQuestions.AddAsync(new SecretQuestion
                        {
                            User = identityUser,
                            Answer = model.Answer,
                            Question = model.SecretQuestion
                        });
                        await _ctx.SaveChangesAsync();
                        return Redirect(returnUrl);
                    }
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                }
                ModelState.AddModelError(nameof(RegistrationViewModel.Email), "Пользователь с такой почтой уже существует");
            }
            return View("Registration", model);
        }

        public IActionResult Login(string returnUrl)
        {
            return View();
        }
    }
}
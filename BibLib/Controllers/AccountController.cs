using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using BibLib.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibLib.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _ctx;
        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, AppDbContext ctx)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _ctx = ctx;
        }

        [HttpGet]
        public IActionResult Registration(string returnUrl)
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Profile", "Profile");
                }
            }
            RegistrationViewModel model = new RegistrationViewModel
            {
                ListOfSecretQuestions = new List<string>
                {
                    "Девичья фамилия матери",
                    "Кличка первого питомца",
                    "Марка первой машины"
                }
            };
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model, string returnUrl)
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Profile", "Profile");
                }
            }
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
        
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Profile", "Profile");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string returnUrl, LoginViewModel model)
        {
            if (User.Identity != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Profile", "Profile");
                }
            }
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user != default)
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, model.RememberMe);
                        return Redirect(returnUrl);
                    }
                    ModelState.AddModelError(nameof(LoginViewModel.Password), "Неверный пароль");
                }
                ModelState.AddModelError(nameof(LoginViewModel.Email), "Пользователя с такой почтой не существует");
            }

            return View("Login", model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    string question = (from DB_question in _ctx.SecretQuestions
                        where DB_question.User == user
                        select DB_question.Question).Single();
                    return SecurityCheck(question);
                }
            }
            return View("ForgotPassword", model);
        }

        private IActionResult SecurityCheck(string question)
        {
            return View("SecurityCheck", new SecurityCheckViewModel{Question = question});
        }
        
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using BibLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibLib.Models.ViewModels;
using BibLib.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace BibLib.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityUser> _roleManager;

        public AdminController(UserManager<IdentityUser> umr, RoleManager<IdentityUser> rlm)
        {
            _userManager = umr;
            _roleManager = rlm;
        }

        [HttpGet]
        public IActionResult Users(int page = 1)
        {
            int USERS_PER_PAGE = 30;
            IQueryable<IdentityUser> users = _userManager.Users;
            var model = new AdminViewModel
            {
                Pages = new PaginationViewModel
                {
                    PageNumber = page,
                    TotalPages = users.Count() / USERS_PER_PAGE
                },
                Users = users.Skip(USERS_PER_PAGE * (page - 1)).Take(USERS_PER_PAGE).Select(x => new UserViewModel
                    {
                        Email = x.Email,
                        Role = SetRole(x).Result
                    }
                ).ToList()
            };
            return View(model);
        }

        private async Task<string> SetRole(IdentityUser user)
        {
            if (await _userManager.IsInRoleAsync(user, Config.AdminRole))
            {
                return Config.AdminRole;
            }

            if (await _userManager.IsInRoleAsync(user, Config.LibrarianRole))
            {
                return Config.LibrarianRole;
            }

            if (await _userManager.IsInRoleAsync(user, Config.PremiumRole))
            {
                return Config.PremiumRole;
            }

            return "";
        }
    }
}
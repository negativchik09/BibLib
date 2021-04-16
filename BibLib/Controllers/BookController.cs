using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibLib.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

// txt fb2 rtf epub

namespace BibLib.Controllers
{
    [Authorize(Roles = "admin,librarian")]
    public class BookController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IHostEnvironment _host; 

        public BookController(AppDbContext ctx, IHostEnvironment host)
        {
            _ctx = ctx;
            _host = host;
        }
        
        // Create

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateOrEdit", new BookCreateOrEditViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrCreate(BookCreateOrEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Author
                // Genre
                // Image
                // Texts
            }
            return View("CreateOrEdit", model);
        }
    }
}
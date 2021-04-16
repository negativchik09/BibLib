using System;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using BibLib.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibLib.Models.ViewModels;
using Microsoft.AspNetCore.Http;
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
            return View("CreateOrEdit", new BookCreateOrEditViewModel{startModel = null});
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> EditOrCreate(BookCreateOrEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.startModel == null)
                {
                    BookBuilder builder = new BookBuilder(_ctx);
                    builder.SetTitle(model.Title);
                    model.Author = model.Author.Trim();
                    await builder.SetAuthor(model.Author);
                    model.Genre = model.Genre.Trim();
                    await builder.SetGenre(model.Genre);
                    builder.SetSeries(model.Series);
                    builder.SetAnnotation(model.Annotation);
                    if (model.Image != null)
                    {
                        if (model.Image.Length == 0)
                        {
                            ModelState.AddModelError(nameof(model.Image), "Ошибка при загрузке, попробуйте ещё раз");
                            return View("CreateOrEdit", model);
                        }
                        await builder.SetImageAsync(model.Image, _host.ContentRootPath);
                    }
                    else
                    {
                        await builder.SetImageAsync(null, _host.ContentRootPath);
                    }
                    if (model.Text == null || model.Text.Length == 0)
                    {
                        ModelState.AddModelError(nameof(model.Text), "Ошибка при загрузке, попробуйте ещё раз");
                        return View("CreateOrEdit", model);
                    }
                    await builder.SetTextAsync(model.Text, _host.ContentRootPath);
                    
                    Book book = builder.GetBook();
                    Console.WriteLine("Объект книги создан");
                    await _ctx.Books.AddAsync(book);
                    foreach (var author in book.Author)
                    {
                        _ctx.Entry(author).State = EntityState.Unchanged;
                    }
                    foreach (var genre in book.Genre)
                    {
                        _ctx.Entry(genre).State = EntityState.Unchanged;
                    }
                    await _ctx.SaveChangesAsync();
                    Console.WriteLine("Запись в БД создана");
                    return Redirect($"~/Book/Info/{book.Id}");
                }
            }
            return View("CreateOrEdit", model);
        }
    }
}
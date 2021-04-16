using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> Edit(int id)
        {
            Book book = await _ctx.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return StatusCode(404);
            }
            BookCreateOrEditViewModel startModel = new BookCreateOrEditViewModel()
            {
                Annotation = book.Annotation,
                Author = string.Join(", ",_ctx.Authors
                    .AsNoTracking()
                    .Where(x => x.Books.Contains(book))
                    .Select(x => x.Name)
                    .ToList()),
                Genre = string.Join(", ",_ctx.Genres
                    .AsNoTracking()
                    .Where(x => x.Books.Contains(book))
                    .Select(x => x.Title)
                    .ToList()),
                Id = book.Id,
                NumberOfPages = book.NumberOfPages,
                Popularity = book.Popularity,
                Rating = book.Rating,
                Series = book.Series,
                Title = book.Title
            };
            BookCreateOrEditViewModel model = new BookCreateOrEditViewModel()
            {
                Annotation = book.Annotation,
                Author = string.Join(", ",_ctx.Authors
                    .AsNoTracking()
                    .Where(x => x.Books.Contains(book))
                    .Select(x => x.Name)
                    .ToList()),
                Genre = string.Join(", ",_ctx.Genres
                    .AsNoTracking()
                    .Where(x => x.Books.Contains(book))
                    .Select(x => x.Title)
                    .ToList()),
                Id = book.Id,
                NumberOfPages = book.NumberOfPages,
                Popularity = book.Popularity,
                Rating = book.Rating,
                Series = book.Series,
                Title = book.Title,
                startModel = startModel
            };
            return View("CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditOrCreate(BookCreateOrEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                BookBuilder builder;
                Book book;
                if (model.Id == 0)
                {
                    builder = new BookBuilder(_ctx);
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

                    if (model.Text == null)
                    {
                        ModelState.AddModelError(nameof(model.Text), "Обязательное поле");
                        return View("CreateOrEdit", model);
                    }

                    if (model.Text.Length == 0)
                    {
                        ModelState.AddModelError(nameof(model.Text), "Ошибка при загрузке, попробуйте ещё раз");
                        return View("CreateOrEdit", model);
                    }

                    await builder.SetTextAsync(model.Text, _host.ContentRootPath);
                    book = builder.GetBook();
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
                    return Redirect($"~/Book/Info/{book.Id}");
                }
                book = await _ctx.Books.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
                builder = new BookBuilder(book, _ctx);
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
                if (model.Text != null)
                {
                    if (model.Text.Length == 0)
                    {
                        ModelState.AddModelError(nameof(model.Text), "Ошибка при загрузке, попробуйте ещё раз");
                        return View("CreateOrEdit", model);
                    }

                    await builder.SetTextAsync(model.Text, _host.ContentRootPath);
                }
                book = builder.GetBook();
                foreach (var author in book.Author)
                {
                    _ctx.Entry(author).State = EntityState.Unchanged;
                }
                foreach (var genre in book.Genre)
                {
                    _ctx.Entry(genre).State = EntityState.Unchanged;
                }
                await _ctx.SaveChangesAsync();
                return Redirect($"~/Book/Info/{book.Id}");
            }
            return View("CreateOrEdit", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Info(int id)
        {
            Book book = await _ctx.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return StatusCode(404);
            }
            book.Popularity++;
            await _ctx.SaveChangesAsync();
            BookInfoViewModel model = new BookInfoViewModel
            {
                Annotation = book.Annotation,
                Author = string.Join(", ",_ctx.Authors
                    .AsNoTracking()
                    .Where(x => x.Books.Contains(book))
                    .Select(x => x.Name)
                    .ToList()),
                Genre = string.Join(", ",_ctx.Genres
                    .AsNoTracking()
                    .Where(x => x.Books.Contains(book))
                    .Select(x => x.Title)
                    .ToList()),
                Id = book.Id,
                Image = book.Image,
                NumberOfPages = book.NumberOfPages,
                Popularity = book.Popularity,
                Rating = book.Rating,
                Series = book.Series,
                Text = book.Text,
                Title = book.Title
            };
            return View("BookInfo", model);
        }
    }
}
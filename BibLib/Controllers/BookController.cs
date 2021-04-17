﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json;

// txt fb2 rtf epub

namespace BibLib.Controllers
{
    [Authorize(Roles = "admin,librarian")]
    public class BookController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IHostEnvironment _host;
        private readonly string _imgBasePath;
        private readonly string _textBasePath;
        private readonly string _imgHTML;
        public BookController(AppDbContext ctx, IHostEnvironment host)
        {
            _ctx = ctx;
            _host = host;
            _imgBasePath = @$"{_host.ContentRootPath.Replace('\\', '/')}/wwwroot/img/books/";
            _textBasePath = @$"{_host.ContentRootPath.Replace('\\', '/')}/wwwroot/texts/";
            _imgHTML = "../../img/books/";
        }
        
        // Create

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateOrEdit", new BookCreateOrEditViewModel{Id = 0});
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            BookDTO book = await _ctx.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return StatusCode(404);
            }

            return View("CreateOrEdit", new BookCreateOrEditViewModel
            {
                Id = 0,
                Annotation = book.Annotation,
                Author = string.Join(", ", _ctx.AuthorBook.AsNoTracking()
                    .Where(x => x.BookId == book.Id)
                    .Select(pair => _ctx.Authors.AsNoTracking()
                        .FirstOrDefault(a => a.Id == pair.AuthorId).Name)),
                Genre = string.Join(", ", _ctx.GenreBook.AsNoTracking()
                    .Where(x => x.BookId == book.Id)
                    .Select(pair => _ctx.Genres.AsNoTracking()
                        .FirstOrDefault(g => g.Id == pair.GenreId).Title)),
                Series = book.Series,
                Title = book.Title,
                Image = null,
                Text = null
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditOrCreate(BookCreateOrEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateOrEdit", model);
            }
            List<string> pages;
            using (var reader = new StreamReader(model.Text.OpenReadStream()))
            {
                pages = BookSlicer(await reader.ReadToEndAsync());
            }
            BookDTO book;
            Author authorConvert;
            Genre genreConvert;
            if (model.Id == 0)
            {
                book = new BookDTO
                {
                    Annotation = model.Annotation,
                    Popularity = 0,
                    Rating = 0,
                    Title = model.Title,
                    Series = model.Series,
                    NumberOfPages = pages.Count
                };
                await _ctx.Books.AddAsync(book);
            }
            else
            {
                book = await _ctx.Books.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (book == null) return StatusCode(404);
                book.Annotation = model.Annotation;
                book.Title = model.Title;
                book.Series = model.Series;
                book.NumberOfPages = pages.Count;
            }
            await _ctx.SaveChangesAsync();
            // Author
            authorConvert = new Author(_ctx);
            string[] authors = model.Author.Trim().Split(',', StringSplitOptions.TrimEntries);
            IEnumerable<AuthorDTO> authorDTOs = authors.Select(x => authorConvert.GetAuthorDTO(x).Result);
            foreach (var author in authorDTOs)
            {
                if (await _ctx.AuthorBook.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.AuthorId == author.Id && x.BookId == book.Id) == null)
                {
                    await _ctx.AuthorBook.AddAsync(new AuthorBook {BookId = book.Id, AuthorId = author.Id});
                }
            }
            // Genres
            genreConvert = new Genre(_ctx);
            string[] genres = model.Genre.Trim().Split(',', StringSplitOptions.TrimEntries);
            IEnumerable<GenreDTO> genreDTOs = genres.Select(x => genreConvert.GetGenreDTO(x).Result);
            foreach (var genre in genreDTOs)
            {
                if (await _ctx.GenreBook.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.GenreId == genre.Id && x.BookId == book.Id) == null)
                {
                    await _ctx.GenreBook.AddAsync(new GenreBook {BookId = book.Id, GenreId = genre.Id});
                }
            }
            await _ctx.SaveChangesAsync();
            // Image
            await SaveImage(model.Image, $@"{_imgBasePath}{book.Id}");
            // Text
            await SaveText(JsonConvert.SerializeObject(pages), @$"{_textBasePath}{book.Id}");
            return RedirectToAction("Info", "Book", book.Id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Info(int id)
        {
            BookDTO book = await _ctx.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return StatusCode(404);
            }
            book.Popularity++;
            await _ctx.SaveChangesAsync();
            BookInfoViewModel model = new BookInfoViewModel
            {
                Annotation = book.Annotation,
                Author = _ctx.AuthorBook.AsNoTracking()
                    .Where(x => x.BookId == book.Id)
                    .Select(pair => _ctx.Authors.AsNoTracking()
                        .FirstOrDefault(a => a.Id == pair.AuthorId))
                    .ToList(),
                Genre = _ctx.GenreBook.AsNoTracking()
                    .Where(x => x.BookId == book.Id)
                    .Select(pair => _ctx.Genres.AsNoTracking()
                        .FirstOrDefault(g => g.Id == pair.GenreId))
                    .ToList(),
                Id = book.Id,
                Image = $"{_imgHTML}{id}/{Path.GetFileName(Directory.GetFiles($"{_imgBasePath}{id}")[0])}",
                NumberOfPages = book.NumberOfPages,
                Rating = book.Rating,
                Series = book.Series,
                Title = book.Title
            };
            return View("BookInfo", model);
        }

        private async Task SaveImage(IFormFile file, string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            if (file == null)
            {
                return;
            }
            await using (var stream = System.IO.File.Create(
                $"{path}/img{Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"'))}"))
            {
                await file.CopyToAsync(stream);
                await stream.FlushAsync();
            }
        }
        
        private async Task SaveText(string pages, string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            await System.IO.File.WriteAllTextAsync($"{path}/pages.json", pages);
        }
        
        private List<string> BookSlicer(string input)
        {
            const int _symbolsInRow = 50;
            const int _maxRowsOnPage = 45;
            string row = "";
            int rowsOnPage = 0;
            string page = "";
            List<string> pages = new List<string>();
            for (int i = 0; i < input.Length; i++)
            {
                row += input[i].ToString();
                if (input[i] == '\r')
                {
                    i++;
                    row += input[i].ToString();
                    page += row;
                    rowsOnPage++;
                    row = "";
                    if (rowsOnPage == _maxRowsOnPage)
                    {
                        pages.Add(page);
                        page = "";
                        rowsOnPage = 0;
                    }
                    continue;
                }
                if (row.Length != _symbolsInRow) continue;
                i++;
                for (;; i++)
                {
                    row += input[i].ToString();
                    if (input[i] == ' ')
                    {
                        break;
                    }
                }
                page += row;
                rowsOnPage++;
                row = "";
                if (rowsOnPage != _maxRowsOnPage) continue;
                i++;
                for (;; i++)
                {
                    page += input[i].ToString();
                    if (input[i] == '\r')
                    {
                        i++;
                        page += input[i].ToString();
                        break;
                    }
                }
                pages.Add(page);
                page = "";
                rowsOnPage = 0;
            }
            return pages;
        }
    }
}
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _umr;
        private readonly string _imgBasePath;
        private readonly string _textBasePath;
        private readonly string _imgHtml;
        public BookController(AppDbContext ctx, IHostEnvironment host, UserManager<IdentityUser> umr)
        {
            _ctx = ctx;
            _umr = umr;
            _imgBasePath = @$"{host.ContentRootPath.Replace('\\', '/')}/wwwroot/img/books/";
            _textBasePath = @$"{host.ContentRootPath.Replace('\\', '/')}/wwwroot/texts/";
            _imgHtml = "../../img/books/";
        }
        
        // Create

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateOrEdit", new BookCreateOrEditViewModel{Id = 0});
        }

        // Edit
        
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
            List<string> pages = new List<string>();
            if (model.Id == 0 && model.Text == null)
            {
                ModelState.AddModelError(nameof(model.Text), "Обязательное поле");
                return View("CreateOrEdit", model);
            }
            if (model.Id == 0 || model.Text != null)
            {
                using (var reader = new StreamReader(model.Text.OpenReadStream()))
                {
                    pages = BookSlicer(await reader.ReadToEndAsync());
                }

                if (model.Id != 0)
                {
                    List<Bookmark> marks = _ctx.Bookmarks.Where(x => x.BookId == model.Id).ToList();
                    foreach (var mark in marks)
                    {
                        mark.IsAvailable = false;
                    }
                }
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
                book.NumberOfPages = pages.Count switch
                {
                    0 => book.NumberOfPages,
                    _ => pages.Count
                };
            }
            await _ctx.SaveChangesAsync();
            // Author
            authorConvert = new Author(_ctx);
            string[] authors = model.Author.Trim().Split(',', StringSplitOptions.TrimEntries);
            IEnumerable<AuthorDTO> authorDTOs = authors.Select(x => authorConvert.GetAuthorDTO(x).Result);
            List<AuthorBook> authorsID = _ctx.AuthorBook.AsNoTracking().Where(x => x.BookId == book.Id).ToList();
            authorsID.RemoveAll(a => !authorDTOs.Select(x => x.Id).Contains(a.AuthorId));
            _ctx.AuthorBook.RemoveRange(authorsID);
            await _ctx.SaveChangesAsync();
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
            List<GenreBook> genresID = _ctx.GenreBook.AsNoTracking().Where(x => x.BookId == book.Id).ToList();
            genresID.RemoveAll(g => genreDTOs.Select(x => x.Id).Contains(g.GenreId));
            _ctx.GenreBook.RemoveRange(genresID);
            await _ctx.SaveChangesAsync();
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
            if (model.Id == 0 || model.Image != null)
            {
                await SaveImage(model.Image, $@"{_imgBasePath}{book.Id}");
            }
            // Text
            if (model.Id == 0 || model.Text != null)
            {
                await SaveText(JsonConvert.SerializeObject(pages), @$"{_textBasePath}{book.Id}");
            }
            return Redirect($"~/Book/Info/{book.Id}");
        }

        // Read
        
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
                Image = $"{_imgHtml}{id}/{Path.GetFileName(Directory.GetFiles($"{_imgBasePath}{id}")[0])}",
                NumberOfPages = book.NumberOfPages,
                Rating = book.Rating,
                Series = book.Series,
                Title = book.Title,
                Bookmarks = new List<BookmarkViewModel>()
            };
            if (User.Identity == null || User.Identity.Name == null)
            {
                return View("BookInfo", model);
            }
            IdentityUser user = await _umr.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return View("BookInfo", model);
            }
            model.Bookmarks = _ctx.Bookmarks
                .Where(x => x.BookId == id && x.User == user)
                .Select(bm => new BookmarkViewModel
                {
                    Id = bm.Id,
                    BookId = id,
                    Page = bm.Page,
                    Name = bm.Name,
                    IsAvailable = bm.IsAvailable
                }).ToList();
            model.IsInFavorites = _ctx.Favorites.Any(x => x.User == user && x.BookId == id);
            Mark mark = await _ctx.Marks.FirstOrDefaultAsync(x => x.User == user && x.BookId == id);
            model.UpARating = mark switch
            {
                null => null,
                _ => mark.UpRating
            };
            return View("BookInfo", model);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Rating(int id, string button)
        {
            IdentityUser user = await _umr.FindByNameAsync(User.Identity?.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (button == "-")
            {
                Mark mark = await _ctx.Marks.FirstOrDefaultAsync(x => x.User == user && x.BookId == id);
                if (mark == null)
                {
                    await _ctx.Marks.AddAsync(new Mark {BookId = id, User = user, UpRating = false});
                }
                else if (mark.UpRating)
                {
                    _ctx.Marks.Remove(mark);
                }
                else
                {
                    return StatusCode(404);
                }
                BookDTO book = await _ctx.Books.FirstOrDefaultAsync(x => x.Id == id);
                book.Rating--;
            }
            else
            {
                Mark mark = await _ctx.Marks.FirstOrDefaultAsync(x => x.User == user && x.BookId == id);
                if (mark == null)
                {
                    await _ctx.Marks.AddAsync(new Mark {BookId = id, User = user, UpRating = true});
                }
                else if (!mark.UpRating)
                {
                    _ctx.Marks.Remove(mark);
                }
                else
                {
                    return StatusCode(404);
                }
                BookDTO book = await _ctx.Books.FirstOrDefaultAsync(x => x.Id == id);
                book.Rating++;
            }
            await _ctx.SaveChangesAsync();
            return Redirect($"~/Book/Info/{id}");
        }
        
        // Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            BookDTO book = await _ctx.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return StatusCode(404);
            }
            _ctx.Books.Remove(book);
            _ctx.AuthorBook.RemoveRange(_ctx.AuthorBook.Where(x => x.BookId == id));
            _ctx.GenreBook.RemoveRange(_ctx.GenreBook.Where(x=>x.BookId == id));
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        
        [AllowAnonymous]
        public IActionResult Read(int id, int page = 1, int font = 14)
        {
            string path = $"{_textBasePath}{id.ToString()}/pages.json";
            List<string> pages = JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText(path));
            string value = pages[page - 1];
            return View("Read", new ReadViewModel
            {
                Text = value, 
                Pages = new PaginationViewModel
                {
                    PageNumber = page, TotalPages = pages.Count
                },
                Id = id,
                FontSize = font
            });
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
            const int _maxRowsOnPage = 60;
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using BibLib.Models;
using BibLib.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;

// Random element
// Random rand = new Random();
// el = _ctx.Books.OrderBy(r => Guid.NewGuid()).Skip(rand.Next(1, _ctx.Books.Count)).Take(1).FirstOrDefault();

namespace BibLib.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly string img_path;

        public HomeController(AppDbContext ctx, IHostEnvironment host)
        {
            _ctx = ctx;
            img_path = @$"{host.ContentRootPath.Replace('\\', '/')}/wwwroot/img/books/";
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string queue)
        {
            return await Search(new SearchViewModel
            {
                GeneralSearch = queue
            });
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchViewModel model, int page = 1)
        {
            int InOnePage = 10;
            
            model.SeriesDataSet ??= _ctx.Books.AsNoTracking().Select(x => x.Series[..(x.Series.LastIndexOf('-') - 1)]).Distinct().ToList();
            model.AuthorDataSet ??= _ctx.Authors.AsNoTracking().Select(x => x.Name).ToList();
            model.GenreDataSet ??= _ctx.Genres.AsNoTracking().Select(x => x.Title).ToList();
            model.TitleDataSet ??= _ctx.Books.AsNoTracking().Select(x => x.Title).ToList();
            
            // Filtration
            IQueryable<BookDTO> books = await GeneralSearch(model.GeneralSearch);
            // Title
            if (!string.IsNullOrEmpty(model.TitleInput.Trim()))
            {
                books = books.Where(x => x.Title.Contains(model.TitleInput.Trim()));
            }
            // Author
            if (!string.IsNullOrEmpty(model.AuthorInput.Trim()))
            {
                var i = _ctx.AuthorBook.AsNoTracking().Where(x => _ctx.Authors.AsNoTracking()
                        .Where(y=>y.Name.Contains(model.AuthorInput.Trim()))
                        .Select(y=>y.Id)
                        .Contains(x.AuthorId))
                    .Select(x => x.BookId);
                books = books.Where(x=>i.Contains(x.Id));
            }
            // Genre
            if (!string.IsNullOrEmpty(model.GenreInput.Trim()))
            {
                var j = _ctx.GenreBook.AsNoTracking().Where(x => _ctx.Genres.AsNoTracking()
                        .Where(y=>y.Title.Contains(model.GenreInput.Trim()))
                        .Select(y=>y.Id)
                        .Contains(x.GenreId))
                    .Select(x => x.BookId);
                books = books.Where(x => j.Contains(x.Id));
            }
            // Series
            if (!string.IsNullOrEmpty(model.SeriesInput.Trim()))
            {
                books = books.Where(x => x.Series.Contains(model.SeriesInput.Trim()));
            }

            // Sorting
            books = model.SortingMethod switch
            {
                SortStates.TitleAsc => books.OrderBy(x => x.Title),
                SortStates.TitleDesc => books.OrderByDescending(x => x.Title),
                SortStates.PopularityAsc => books.OrderBy(x => x.Popularity),
                SortStates.PopularityDesc => books.OrderByDescending(x => x.Popularity),
                SortStates.RatingAsc => books.OrderBy(x => x.Rating),
                SortStates.RatingDesc => books.OrderByDescending(x => x.Rating),
                SortStates.SeriesAsc => books.OrderBy(x => x.Series),
                SortStates.SeriesDesc => books.OrderByDescending(x => x.Series),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            // Pagination
            model.Pages.TotalPages = (int)Math.Ceiling(books.Count() / (double)InOnePage);
            model.Pages.PageNumber = page;
            model.List = books
                .Skip((page - 1) * InOnePage)
                .Take(InOnePage)
                .Select(book=>new ShortBookViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
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
                    Image = $"../../img/books/{book.Id}/{Path.GetFileName(Directory.GetFiles($"{img_path}{book.Id}")[0])}",
                    NumberOfPages = book.NumberOfPages,
                    Popularity = book.Popularity,
                    Rating = book.Rating,
                    Series = book.Series
                })
                .ToList();
            return View("Search", model);
        }
        
        private async Task<IQueryable<BookDTO>> GeneralSearch(string queue)
        {
            IQueryable<BookDTO> books = _ctx.Books.AsNoTracking();
            if (string.IsNullOrEmpty(queue)) return books;
            // author
            var i = _ctx.AuthorBook.AsNoTracking().Where(x => _ctx.Authors.AsNoTracking()
                    .Where(y=>y.Name.Contains(queue))
                    .Select(y=>y.Id)
                    .Contains(x.AuthorId))
                .Select(x => x.BookId);
            // genre
            var j = _ctx.GenreBook.AsNoTracking().Where(x => _ctx.Genres.AsNoTracking()
                    .Where(y=>y.Title.Contains(queue))
                    .Select(y=>y.Id)
                    .Contains(x.GenreId))
                .Select(x => x.BookId);
            books = books.Where(x =>
                x.Series.Contains(queue) || x.Title.Contains(queue) || i.Contains(x.Id) || j.Contains(x.Id));
            return books;
        }
    }
}
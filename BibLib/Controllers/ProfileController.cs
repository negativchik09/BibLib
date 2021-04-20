using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using BibLib.Models.ViewModels;
using BibLib.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace BibLib.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<IdentityUser> _urm;
        private readonly string img_path;

        public ProfileController(AppDbContext ctx, UserManager<IdentityUser> urm, IHostEnvironment host)
        {
            _ctx = ctx;
            _urm = urm;
            img_path = @$"{host.ContentRootPath.Replace('\\', '/')}/wwwroot/img/books/";
        }
        public async Task<IActionResult> Profile(int page = 1)
        {
            int _booksOnPage = 10;
            IdentityUser user = await _urm.FindByEmailAsync(User.Identity?.Name);
            List<string> roles = new List<string>(await _urm.GetRolesAsync(user));
            AccountInformationViewModel account = new AccountInformationViewModel
            {
                Email = user.Email,
                Status = string.Join(", ", roles)
            };
            IQueryable<int> bookId = _ctx.Favorites.AsNoTracking().Where(x => x.User == user).Select(b => b.BookId);
            List<BookDTO> books = _ctx.Books.AsNoTracking().Where(x => bookId.Contains(x.Id)).ToList();
            await _ctx.SaveChangesAsync();
            List<ShortBookViewModel> list = books.Skip((page - 1)*_booksOnPage)
                .Take(_booksOnPage).Select(bookDto => new ShortBookViewModel
            {
                Id = bookDto.Id,
                Title = bookDto.Title,
                Annotation = bookDto.Annotation,
                Author = _ctx.AuthorBook.AsNoTracking()
                    .Where(x => x.BookId == bookDto.Id)
                    .Select(pair => _ctx.Authors.AsNoTracking()
                        .FirstOrDefault(a => a.Id == pair.AuthorId))
                    .ToList(),
                Genre = _ctx.GenreBook.AsNoTracking()
                    .Where(x => x.BookId == bookDto.Id)
                    .Select(pair => _ctx.Genres.AsNoTracking()
                        .FirstOrDefault(g => g.Id == pair.GenreId))
                    .ToList(),
                Image = $"../../img/books/{bookDto.Id}/{Path.GetFileName(Directory.GetFiles($"{img_path}{bookDto.Id}")[0])}",
                NumberOfPages = bookDto.NumberOfPages,
                Popularity = bookDto.Popularity,
                Rating = bookDto.Rating,
                Series = bookDto.Series
            }).ToList();
            return View("Profile", new ProfileViewModel 
                {
                    Account = account, 
                    Favorites = list,
                    Pages = new PaginationViewModel
                    {
                        PageNumber = page, 
                        TotalPages = list.Select(x => x.Id).Distinct().Count()
                    }
                });
        }
        
        public async Task<IActionResult> AddFavourite(int id)
        {
            IdentityUser user = await _urm.FindByNameAsync(User.Identity?.Name);
            Favorite fav = await _ctx.Favorites
                .FirstOrDefaultAsync(x => x.BookId == id && x.User.Id == user.Id);
            if (fav == null)
            {
                await _ctx.Favorites.AddAsync(new Favorite{BookId = id, User = user});
            }
            await _ctx.SaveChangesAsync();
            return RedirectToRoute(new {controller = "Book", action = "Info", id = $"{id}"});
        }
        public async Task<IActionResult> DeleteFavourite(int id, bool isInProfile = true)
        {
            IdentityUser user = await _urm.FindByNameAsync(User.Identity?.Name);
            Favorite fav = await _ctx.Favorites.FirstOrDefaultAsync(x => x.User.Id == user.Id && x.BookId == id);
            if (fav != null)
            {
                _ctx.Favorites.Remove(fav);
                await _ctx.SaveChangesAsync();
            }

            if (isInProfile)
            {
                return RedirectToAction("Profile");
            }
            return RedirectToRoute(new {controller = "Book", action = "Info", id = $"{id}"}); 
        }
        
        public async Task<IActionResult> AddBookmark(int id, int page, int font, string name = null)
        {
            IdentityUser user = await _urm.FindByNameAsync(User.Identity?.Name);
            if (await _urm.IsInRoleAsync(user, Config.AdminRole) ||
                await _urm.IsInRoleAsync(user, Config.LibrarianRole) ||
                await _urm.IsInRoleAsync(user, Config.PremiumRole))
            {
                await _ctx.Bookmarks.AddAsync(new Bookmark
                {
                    BookId = id,
                    IsAvailable = true,
                    Page = page,
                    Name = name,
                    User = user
                });
                await _ctx.SaveChangesAsync();
            }
            else
            {
                Bookmark bookmark = await _ctx.Bookmarks.FirstOrDefaultAsync(x => x.BookId == id && x.User == user);
                if (bookmark == null)
                {
                    await _ctx.Bookmarks.AddAsync(new Bookmark
                    {
                        BookId = id,
                        IsAvailable = true,
                        Page = page,
                        User = user
                    });
                    await _ctx.SaveChangesAsync();
                }
                else
                {
                    bookmark.Page = page;
                    await _ctx.SaveChangesAsync();
                }
            }
            return Redirect($"~/Book/Read/1?page={page}&font={font}");
        }

        public async Task<IActionResult> Bookmarks(int page = 1, string search = "")
        {
            int _booksOnPage = 10;
            IdentityUser user = await _urm.FindByNameAsync(User.Identity.Name);
            List<Bookmark> list = _ctx.Bookmarks.Where(x => x.User == user).ToList();
            if (search == "")
            {
                page = 1;
                _booksOnPage = list.Select(x => x.Id).Distinct().Count();
            }
            AllBookmarksViewModel model = new AllBookmarksViewModel
            {
                List = new List<(string BookName, List<BookmarkViewModel> Bookmarks)>(),
                Pages = new PaginationViewModel{
                    PageNumber = page,
                    TotalPages = (int)Math.Ceiling(list.Select(x => x.Id).Distinct().Count() / (double)_booksOnPage)
                }
            };
            foreach (var Id in list.Select(x=>x.BookId).Distinct().Skip((page - 1)*_booksOnPage)
                .Take(_booksOnPage))
            {
                model.List
                    .Add(new ((await _ctx.Books.FirstOrDefaultAsync(x=> x.Id == Id)).Title,
                        list.Where(x=>x.BookId == Id && (search == "" || x.Name.Contains(search)))
                        .Select(x => new BookmarkViewModel
                        {
                            Id = x.Id,
                            BookId = x.BookId,
                            IsAvailable = x.IsAvailable,
                            Name = x.Name,
                            Page = x.Page
                        })
                        .ToList()
                    ));
            }
            return View("Bookmarks", model);
        }

        public async Task<IActionResult> DeleteBookmark(int id)
        {
            IdentityUser user = await _urm.FindByNameAsync(User.Identity.Name);
            Bookmark mark = await _ctx.Bookmarks.FirstOrDefaultAsync(x => x.Id == id && x.User == user);
            if (mark != null)
            {
                _ctx.Bookmarks.Remove(mark);
                await _ctx.SaveChangesAsync();
            }

            return StatusCode(200);
        }
    }
}
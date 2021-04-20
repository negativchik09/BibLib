using System.Collections.Generic;
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

namespace BibLib.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<IdentityUser> _urm;

        public ProfileController(AppDbContext ctx, UserManager<IdentityUser> urm)
        {
            _ctx = ctx;
            _urm = urm;
        }
        public async Task<IActionResult> Profile()
        {
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
            List<ShortBookViewModel> list = books.Select(bookDto => new ShortBookViewModel
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
                Image = $"../../img/books/{bookDto.Id}",
                NumberOfPages = bookDto.NumberOfPages,
                Popularity = bookDto.NumberOfPages,
                Rating = bookDto.NumberOfPages,
                Series = bookDto.Series
            }).ToList();
            return View("Profile", new ProfileViewModel {Account = account, Favorites = list});
        }

        // ////////////////////////////////////////////
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
        // ////////////////////////////////////////////
        public async void AddBookmark(int id, int page, int font, string name = null)
        {
            IdentityUser user = await _urm.FindByNameAsync(User.Identity?.Name);
            if (await _ctx.Favorites.AnyAsync(x => x.BookId == id && x.User == user) 
                || await _urm.IsInRoleAsync(user, Config.AdminRole) 
                || await _urm.IsInRoleAsync(user, Config.LibrarianRole) 
                || await _urm.IsInRoleAsync(user, Config.PremiumRole))
            {
                await _ctx.Favorites.AddAsync(new Favorite{BookId = id, User = user});
            }
            else
            {
                (await _ctx.Bookmarks.FirstAsync(x => x.BookId == id && x.User == user)).Page = page;
            }

            await _ctx.SaveChangesAsync();
        }
    }
}
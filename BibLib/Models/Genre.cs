using System;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibLib.Models
{
    public class Genre
    {
        private readonly AppDbContext _ctx;

        public Genre(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<GenreDTO> GetGenreDTO(string input)
        {
            string genreName = LetterCasing(input.Trim());
            GenreDTO genre = await _ctx.Genres.FirstOrDefaultAsync(x => x.Title == genreName);
            if (genre != null) return genre;
            genre = new GenreDTO {Title = genreName};
            await _ctx.AddAsync(genre);
            await _ctx.SaveChangesAsync();
            return genre;
        }

        private string LetterCasing(string input)
        {
            return $"{input[0].ToString().ToUpper()}{input[1..].ToString().ToLower()}";
        }
    }
}
using System;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibLib.Models
{
    public class Author
    {
        private readonly AppDbContext _ctx;

        public Author(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<AuthorDTO> GetAuthorDTO(string input)
        {
            string authorName = LetterCasing(input.Trim());
            AuthorDTO author = await _ctx.Authors.FirstOrDefaultAsync(x => x.Name == authorName);
            if (author != null) return author;
            author = new AuthorDTO {Name = authorName};
            await _ctx.AddAsync(author);
            await _ctx.SaveChangesAsync();
            return author;
        }

        private string LetterCasing(string input)
        {
            string[] buffer = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int index;
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = $"{buffer[i][0].ToString().ToUpper()}{buffer[i][1..].ToString().ToLower()}";
                index = buffer[i].IndexOf('-');
                if (index + 2 < buffer[i].Length && index != -1)
                {
                    buffer[i] = $"{buffer[i][0..(index + 1)]}{buffer[i][index + 1].ToString().ToUpper()}{buffer[i][(index + 2)..]}";
                }
            }

            return string.Join(' ', buffer);
        }
    }
}
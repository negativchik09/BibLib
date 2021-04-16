using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BibLib.Domain;
using BibLib.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BibLib.Models
{
    public class BookBuilder
    {
        private Book _book;
        private string _guid;
        private readonly AppDbContext _ctx;

        public BookBuilder(AppDbContext ctx)
        {
            _book = new Book
            {
                Author = new List<Author>(),
                Genre = new List<Genre>(),
                Id = 0,
                Image = null,
                NumberOfPages = 0,
                Popularity = 0,
                Rating = 0,
                Series = null,
                Text = null,
                Title = null,
            };
            _guid = Guid.NewGuid().ToString();
            _ctx = ctx;
        }

        public BookBuilder(Book book, AppDbContext ctx)
        {
            _book = book;
            _guid = book.Image[(book.Image.LastIndexOf('/'))..(book.Image.LastIndexOf('.'))];
            _ctx = ctx;
        }

        public async Task SetAuthor(string newAuthor)
        {
            List<Author> authors;
            string[] authorsRaw = newAuthor.Split(',');
            authors = new List<Author>();
            Author buffer;
            string optimalString;
            foreach (var author in authorsRaw)
            {
                optimalString = LetterCasing(author);
                buffer = await _ctx.Authors.AsNoTracking().FirstOrDefaultAsync(x => x.Name == optimalString);
                if (buffer == null)
                {
                    buffer = new Author {Name = optimalString};
                    await _ctx.Authors.AddAsync(buffer);
                    await _ctx.SaveChangesAsync();
                }

                authors.Add(buffer);
            }

            _book.Author = authors;
        }

        public async Task SetGenre(string newGenre)
        {
            List<Genre> genres;
            string[] genresRaw = newGenre.Split(',');
            genres = new List<Genre>();
            Genre buffer;
            string optimalString;
            foreach (var genre in genresRaw)
            {
                optimalString = LetterCasing(genre);
                buffer = await _ctx.Genres.FirstOrDefaultAsync(x => x.Title == optimalString);
                if (buffer == null)
                {
                    buffer = new Genre {Title = optimalString};
                    await _ctx.Genres.AddAsync(buffer);
                    _ctx.SaveChanges();
                }

                genres.Add(buffer);
            }

            _book.Genre = genres;
        }

        public void SetTitle(string title)
        {
            _book.Title = title;
        }

        public async Task SetImageAsync(IFormFile file, string rootPath)
        {
            if (file == null)
            {
                _book.Image = $"../../img/DefaultCover.jpeg";
                return;
            }
            string path =
                @$"{rootPath.Replace('\\', '/')}/wwwroot/img/{_guid}{Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"'))}";
            await using (var stream = File.Create(path))
            {
                await file.CopyToAsync(stream);
                await stream.FlushAsync();
            }

            _book.Image =
                $"../../img/{_guid}{Path.GetExtension(ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName?.Trim('"'))}";
        }

        public async Task SetTextAsync(IFormFile file, string rootPath)
        {
            StreamReader reader = new StreamReader(file.OpenReadStream());
            string text = await reader.ReadToEndAsync();
            Directory.CreateDirectory($"{rootPath.Replace('\\', '/')}/wwwroot/texts/{_guid}/");
            await File.WriteAllTextAsync($"{rootPath.Replace('\\', '/')}/wwwroot/texts/{_guid}/origin.txt", text);
            reader.Close();
            List<string> pages = BookSlicer(text).ToList();
            for (int i = 1; i < pages.Count + 1; i++)
            {
                await using (var stream = File.Create($"{rootPath.Replace('\\', '/')}/wwwroot/texts/{_guid}/{i}.txt"))
                {
                    StreamWriter streamWriter = new StreamWriter(stream);
                    streamWriter.Write(pages[i-1]);
                    streamWriter.Close();
                }
            }
            _book.NumberOfPages = pages.Count;
            _book.Text = $"../../texts/{_guid}/";
        }

        public void SetSeries(string series)
        {
            _book.Series = series;
        }

        public void SetAnnotation(string description)
        {
            _book.Annotation = description;
        }

        public Book GetBook()
        {
            return _book;
        }

        private static Queue<string> BookSlicer(string input)
        {
            const int _symbolsInRow = 50;
            const int _maxRowsOnPage = 45;
            string row = "";
            int rowsOnPage = 0;
            string page = "";
            Queue<string> pages = new Queue<string>();
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
                        pages.Enqueue(page);
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
                pages.Enqueue(page);
                page = "";
                rowsOnPage = 0;
            }
            return pages;
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
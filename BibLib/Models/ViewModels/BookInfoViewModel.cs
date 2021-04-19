using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BibLib.Domain.Entities;

namespace BibLib.Models.ViewModels
{
    public class BookInfoViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Название: ")]
        public string Title { get; set; }
        [Display(Name = "Автор: ")]
        public List<AuthorDTO> Author { get; set; }
        public string Image { get; set; }
        [Display(Name = "Жанр: ")]
        public List<GenreDTO> Genre { get; set; }
        [Display(Name = "Серия: ")]
        public string Series { get; set; }
        [Display(Name = "Аннотация: ")]
        public string Annotation { get; set; }
        [Display(Name = "Кол-во страниц: ")]
        public int NumberOfPages { get; set; }
        [Display(Name = "Рейтинг: ")]
        public int Rating { get; set; }
        public bool IsInFavorites { get; set; }
        public bool? UpARating { get; set; }
        public List<BookmarkViewModel> Bookmarks { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BibLib.Models.ViewModels
{
    public class BookCreateOrEditViewModel
    {
        // hidden
        public BookCreateOrEditViewModel startModel { get; set; }
        // hidden
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Автор")]
        public string Author { get; set; }
        [Display(Name = "Фотография обложки")]
        public IFormFile Image { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Жанр")]
        public string Genre { get; set; }
        [Display(Name = "Серия")]
        public string Series { get; set; }
        public int NumberOfPages { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Текст книги")]
        public IFormFile Text { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
    }
}
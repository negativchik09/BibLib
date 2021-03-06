using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BibLib.Models.ViewModels
{
    public class BookCreateOrEditViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [RegexValidator("[a-zA-ZА-Яа-я-, ]")]
        [Display(Name = "Автор")]
        public string Author { get; set; }
        [Display(Name = "Фотография обложки")]
        public IFormFile Image { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [RegexValidator("[a-zA-ZА-Яа-я, ]")]
        [Display(Name = "Жанр")]
        public string Genre { get; set; }
        [Display(Name = "Серия")]
        public string Series { get; set; }
        [Display(Name = "Текст книги")]
        public IFormFile Text { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Аннотация")]
        public string Annotation { get; set; }
    }
}
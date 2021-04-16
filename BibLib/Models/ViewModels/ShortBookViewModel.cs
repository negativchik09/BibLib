﻿using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class ShortBookViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Автор")]
        public string Author { get; set; }
        [Display(Name = "Фотография обложки")]
        public string Image { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Жанр")]
        public string Genre { get; set; }
        [Display(Name = "Серия")]
        public string Series { get; set; }
        public int NumberOfPages { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class BookInfoViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Title { get; set; }
        [Display(Name = "Автор")]
        public string Author { get; set; }
        public string Image { get; set; }
        [Display(Name = "Жанр")]
        public string Genre { get; set; }
        [Display(Name = "Серия")]
        public string Series { get; set; }
        public string Annotation { get; set; }
        public string Text { get; set; }
        public int NumberOfPages { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
    }
}
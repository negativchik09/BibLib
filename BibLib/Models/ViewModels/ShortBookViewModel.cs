using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BibLib.Domain.Entities;

namespace BibLib.Models.ViewModels
{
    public class ShortBookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<AuthorDTO> Author { get; set; }
        public string Image { get; set; }
        public List<GenreDTO> Genre { get; set; }
        public string Series { get; set; }
        public int NumberOfPages { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
        [Display (Name="Аннотация")]
        public string Annotation { get; set; }
    }
}
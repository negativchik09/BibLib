using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace BibLib.Models.ViewModels
{
    public class BookCreateOrEditViewModel
    {
        // hidden
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public IFormFile Image { get; set; }
        public string Genre { get; set; }
        public string Series { get; set; }
        public int NumberOfPages { get; set; }
        public IFormFile Text { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
    }
}
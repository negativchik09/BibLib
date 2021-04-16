using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace BibLib.Domain.Entities
{
    [Table("Books")]
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public List<Author> Author { get; set; }
        public List<Genre> Genre { get; set; }
        public string Series { get; set; }
        public int NumberOfPages { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
        public string Annotation { get; set; }
    }
}
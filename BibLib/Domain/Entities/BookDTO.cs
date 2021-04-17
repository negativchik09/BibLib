using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace BibLib.Domain.Entities
{
    [Table("Books")]
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Series { get; set; }
        public int NumberOfPages { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
        public string Annotation { get; set; }
        public List<AuthorBook> Authors { get; set; }
        public List<GenreBook> Genres { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibLib.Domain.Entities
{
    [Table("Genres")]
    public class Genre
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Book> Books { get; set; }
    }
}
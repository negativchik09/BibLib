using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibLib.Domain.Entities
{
    [Table("Authors")]
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public List<Book> Books { get; set; }
        public int Rating { get; set; }
        public int Popularity { get; set; }
    }
}
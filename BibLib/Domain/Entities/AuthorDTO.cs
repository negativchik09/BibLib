using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibLib.Domain.Entities
{
    [Table("Authors")]
    public class AuthorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<AuthorBook> Books { get; set; }
    }
}
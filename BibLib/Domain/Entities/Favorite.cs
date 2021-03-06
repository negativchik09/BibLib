using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BibLib.Domain.Entities
{
    [Table("Favorites")]
    public class Favorite
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
    }
}
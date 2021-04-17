using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BibLib.Domain.Entities
{
    [Table("Bookmarks")]
    public class Bookmark
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IdentityUser User { get; set; }
        [ForeignKey("Books")]
        public int BookId { get; set; }
        public int Page { get; set; } 
    }
}
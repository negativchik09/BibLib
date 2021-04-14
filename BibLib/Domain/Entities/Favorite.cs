using Microsoft.AspNetCore.Identity;

namespace BibLib.Domain.Entities
{
    public class Favorite
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public Book Book { get; set; }
    }
}
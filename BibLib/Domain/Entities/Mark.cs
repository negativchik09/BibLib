using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BibLib.Domain.Entities
{
    public class Mark
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public bool UpRating { get; set; }
        public int BookId { get; set; }
    }
}
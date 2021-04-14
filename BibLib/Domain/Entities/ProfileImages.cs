using Microsoft.AspNetCore.Identity;

namespace BibLib.Domain.Entities
{
    public class ProfileImages
    {
        public IdentityUser User { get; set; }
        public string ImagePath { get; set; }
    }
}
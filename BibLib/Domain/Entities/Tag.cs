using Microsoft.AspNetCore.Identity;

namespace BibLib.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IdentityUser User { get; set; }
        public string PagePath { get; set; }
    }
}
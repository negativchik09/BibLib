using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibLib.Domain.Entities
{
    public class SecretQuestion
    {
        public int Id { get; set; }
        public IdentityUser User { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
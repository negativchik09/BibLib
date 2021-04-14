using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BibLib.Domain.Entities
{
    [Keyless]
    public class SecretQuestion
    {
        public IdentityUser User { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
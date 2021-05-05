using System.Collections.Generic;
using BibLib.Service;

namespace BibLib.Models.ViewModels
{
    public class AdminViewModel
    {
        public List<UserViewModel> Users { get; set; }

        public List<string> Roles => _roles;

        private readonly List<string> _roles = new() {Config.PremiumRole, Config.LibrarianRole, Config.AdminRole};
        
        public string SearchQuery { get; set; }
        public PaginationViewModel Pages { get; set; }
    }
}
using System.Collections.Generic;

namespace BibLib.Models.ViewModels
{
    public class ProfileViewModel
    {
        public AccountInformationViewModel Account { get; set; }
        public List<ShortBookViewModel> Favorites { get; set; }
        public PaginationViewModel Pages { get; set; }
    }
}
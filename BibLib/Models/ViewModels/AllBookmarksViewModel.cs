using System.Collections.Generic;

namespace BibLib.Models.ViewModels
{
    public class AllBookmarksViewModel
    {
        public bool Search { get; set; }
        
        public List<(string BookName, List<BookmarkViewModel> Bookmarks)> List { get; set; }
        
        public PaginationViewModel Pages { get; set; }
    }
}
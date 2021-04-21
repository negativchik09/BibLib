using System.Collections.Generic;

namespace BibLib.Models.ViewModels
{
    public class IndexPageViewModel
    {
        public ShortBookViewModel RandomBook { get; set; }
        public List<ShortBookViewModel> MostPopularBooks { get; set; } 
    }
}
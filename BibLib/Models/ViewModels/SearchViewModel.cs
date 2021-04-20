using System.Collections.Generic;
namespace BibLib.Models.ViewModels
{
    public class SearchViewModel
    {
        public List<ShortBookViewModel> List { get; set; }
        public string GeneralSearch { get; set; }
        public string TitleInput { get; set; }
        public List<string> TitleDataSet { get; set; }
        public string AuthorInput { get; set; }
        public List<string> AuthorDataSet { get; set; }
        public string GenreInput { get; set; }
        public List<string> GenreDataSet { get; set; }
        public string SeriesInput { get; set; }
        public List<string> SeriesDataSet { get; set; }
        public PaginationViewModel Pages { get; set; }
        public SortStates SortingMethod { get; set; }
    }
}
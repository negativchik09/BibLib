using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public class SearchViewModel
    {
        public List<ShortBookViewModel> List { get; set; }
        [Display(Name = "Поиск без категории")]
        public string GeneralSearch { get; set; }
        [Display(Name = "Название")]
        public string TitleInput { get; set; }
        public List<string> TitleDataSet { get; set; }
        [Display(Name = "Автор")]
        public string AuthorInput { get; set; }
        public List<string> AuthorDataSet { get; set; }
        [Display(Name = "Жанр")]
        public string GenreInput { get; set; }
        public List<string> GenreDataSet { get; set; }
        [Display(Name = "Серия")]
        public string SeriesInput { get; set; }
        public List<string> SeriesDataSet { get; set; }
        public PaginationViewModel Pages { get; set; }
        public SortStates SortingMethod { get; set; }
    }
}
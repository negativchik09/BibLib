using System.ComponentModel.DataAnnotations;

namespace BibLib.Models.ViewModels
{
    public enum SortStates
    {
        [Display(Name = "По названию по возрастанию")]
        TitleAsc,
        [Display(Name = "По названию по убыванию")]
        TitleDesc,
        [Display(Name = "По популярности по возрастанию")]
        PopularityAsc,
        [Display(Name = "По популярности по убыванию")]
        PopularityDesc,
        [Display(Name = "По рейтингу по возрастанию")]
        RatingAsc,
        [Display(Name = "По рейтингу по убыванию")]
        RatingDesc,
        [Display(Name = "По серии по возрастанию")]
        SeriesAsc,
        [Display(Name = "По серии по убыванию")]
        SeriesDesc
    }
}
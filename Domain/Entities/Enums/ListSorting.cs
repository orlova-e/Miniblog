using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Enums
{
    public enum ListSorting
    {
        [Display(Name = "Most liked")]
        MostLiked,
        [Display(Name = "New first")]
        NewFirst,
        [Display(Name = "Old first")]
        OldFirst,
        [Display(Name = "Alphabetically")]
        Alphabetically,
        [Display(Name = "Alphabetically descending")]
        AlphabeticallyDescending
    }
}

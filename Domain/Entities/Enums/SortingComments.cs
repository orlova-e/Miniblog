using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Enums
{
    public enum SortingComments
    {
        [Display(Name = "Most liked")]
        MostLiked,
        [Display(Name = "New first")]
        NewFirst,
        [Display(Name = "Old first")]
        OldFirst
    }
}

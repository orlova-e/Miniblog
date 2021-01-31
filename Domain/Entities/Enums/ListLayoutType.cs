using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Enums
{
    public enum ListLayoutType
    {
        [Display(Name = "Row")]
        Row,
        [Display(Name = "Grid")]
        Grid
    }
}

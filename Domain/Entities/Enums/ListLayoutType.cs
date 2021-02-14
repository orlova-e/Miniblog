using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Enums
{
    public enum ListLayoutType
    {
        [Display(Name = "Rows")]
        Rows,
        [Display(Name = "Grid")]
        Grid
    }
}

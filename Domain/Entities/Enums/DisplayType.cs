using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Enums
{
    public enum DisplayType
    {
        [Display(Name = "Full text")]
        FullText,
        [Display(Name = "Preview")]
        Preview
    }
}

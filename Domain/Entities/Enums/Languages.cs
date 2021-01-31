using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Enums
{
    public enum Languages
    {
        [Display(Name = "English")]
        English,
        [Display(Name = "Русский")]
        Russian,
        [Display(Name = "Based on user's culture")]
        BasedOnUserCulture
    }
}

using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class CommentAnonymousViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "You must enter your email address here")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [MinLength(4, ErrorMessage = "The minimum length is 4 characters")]
        [MaxLength(25, ErrorMessage = "The maximum length is 25 characters")]
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [Required]
        [MinLength(150, ErrorMessage = "At least 150 characters")]
        public string Text { get; set; }
        [Required]
        public string ArticleId { get; set; }
    }
}

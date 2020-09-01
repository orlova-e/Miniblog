using System.ComponentModel.DataAnnotations;

namespace Miniblog.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

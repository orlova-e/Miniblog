using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(4, ErrorMessage = "The minimum length is 4 characters")]
        [MaxLength(25, ErrorMessage = "The maximum length is 25 characters")]
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The minimum length is 8 characters")]
        public string Password { get; set; }

        public static explicit operator Account(LoginViewModel loginViewModel) => new Account
        {
            Username = loginViewModel.Username,
            Password = loginViewModel.Password
        };
    }
}

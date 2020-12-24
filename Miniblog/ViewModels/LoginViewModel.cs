using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Miniblog.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [DataType(DataType.Text)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public static explicit operator Account(LoginViewModel loginViewModel) => new Account
        {
            Username = loginViewModel.Username,
            Password = loginViewModel.Password
        };
    }
}

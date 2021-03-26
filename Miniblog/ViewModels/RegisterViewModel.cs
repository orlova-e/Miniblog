using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(4, ErrorMessage = "The minimum length is 4 characters")]
        [MaxLength(25, ErrorMessage = "The maximum length is 25 characters")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "You must enter your email address here")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The minimum length is 8 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password confirmation is required")]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The minimum length is 8 characters")]
        [Compare(nameof(Password), ErrorMessage = "The value must be identical to the password")]
        public string PasswordConfirmation { get; set; }

        public static explicit operator Account(RegisterViewModel registerViewModel) => new Account
        {
            Email = registerViewModel.Email,
            Username = registerViewModel.Username,
            Password = registerViewModel.Password
        };
    }
}

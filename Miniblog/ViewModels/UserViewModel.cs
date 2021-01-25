using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using Web.Infrastructure.Validation;

namespace Web.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The minimum length is 8 characters")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The minimum length is 8 characters")]
        public string OldPassword { get; set; }
        public byte[] Avatar { get; set; }
        [MaxFileSize(1 * 1024 * 1024), AllowedFileExtensions("jpg", "jpeg", "png")]
        public IFormFile FormFile { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Description { get; set; }

        public static User operator +(User user, UserViewModel userViewModel)
        {
            user.Email = userViewModel.Email;
            user.Avatar = userViewModel.Avatar;
            user.FullName = userViewModel.FullName;
            user.City = userViewModel.City;
            user.Description = userViewModel.Description;

            return user;
        }

        public static explicit operator UserViewModel(User user)
            => new UserViewModel
            {
                Username = user.Username,
                Email = user.Email,
                Avatar = user.Avatar,
                FullName = user.FullName,
                City = user.City,
                Description = user.Description
            };

        public static explicit operator Account(UserViewModel userViewModel)
            => new Account
            {
                Username = userViewModel.Username,
                Email = userViewModel.Email,
                Password = userViewModel.Password
            };
    }
}

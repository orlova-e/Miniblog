using Domain.Entities;
using System;

namespace Web.Configuration
{
    public class UserData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTimeOffset DateOfRegistration { get; set; }
        public Role Role { get; set; }

        public static implicit operator User(UserData userData)
            => new()
            {
                Username = userData.Username,
                Email = userData.Email,
                Hash = userData.Password,
                DateOfRegistration = userData.DateOfRegistration,
                Role = userData.Role
            };
    }
}

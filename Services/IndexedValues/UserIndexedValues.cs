using Domain.Entities;
using System;

namespace Services.IndexedValues
{
    public class UserIndexedValues : IndexedObject
    {
        public string Username { get; set; }
        public string Description { get; set; }

        public static explicit operator UserIndexedValues(User user)
            => new UserIndexedValues
            {
                Id = user.Id,
                Username = user.Username,
                Description = user.Description
            };

        public override int Rate(string propertyName) => propertyName switch
        {
            nameof(Username) => 5,
            nameof(Description) => 2,
            _ => throw new NotImplementedException()
        };
    }
}

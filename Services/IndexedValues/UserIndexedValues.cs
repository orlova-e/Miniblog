using Domain.Entities;

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
                TypeOfIndexed = user.GetType(),
                Username = user.Username,
                Description = user.Description
            };
    }
}

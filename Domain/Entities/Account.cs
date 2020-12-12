using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [NotMapped]
    public class Account
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

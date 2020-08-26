using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Hash { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public DateTimeOffset DateOfRegistration { get; set; }
        public string City { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Article> Articles { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Tag> Tags { get; set; }
        public User()
        {
            Comments = new List<Comment>();
            Articles = new List<Article>();
            Topics = new List<Topic>();
            Tags = new List<Tag>();
        }
    }
}

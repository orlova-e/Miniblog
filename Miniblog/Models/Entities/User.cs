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
        [Required, MinLength(4), MaxLength(25)]
        public string Username { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Hash { get; set; }
        private string password;
        [NotMapped, DataType(DataType.Password)]
        public string Password { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public DateTimeOffset DateOfRegistration { get; set; }
        public string City { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Article> Articles { get; set; }
        //public List<Article> UserFavourite { get; set; }
        //public List<Article> UserBookmarks { get; set; }
        public List<Topic> Topics { get; set; }
        //public List<Tag> Tags { get; set; }
        public User()
        {
            Comments = new List<Comment>();
            Articles = new List<Article>();
            Topics = new List<Topic>();
            //Tags = new List<Tag>();
        }


    }
}

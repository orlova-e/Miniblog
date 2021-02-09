using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required, MinLength(4), MaxLength(25)]
        //[RegularExpression(@"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$")]
        public string Username { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Hash { get; set; }
        public byte[] Avatar { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public DateTimeOffset DateOfRegistration { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public List<User> Subscribers { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Article> Articles { get; set; }
        public List<UserFavourite> Liked { get; set; }
        public List<UserBookmark> Bookmarked { get; set; }
        public List<CommentLikes> LikedComments { get; set; }
        public List<Topic> Topics { get; set; }
        public List<Tag> Tags { get; set; }
        public User()
        {
            Subscribers = new List<User>();
            Comments = new List<Comment>();
            Articles = new List<Article>();
            Topics = new List<Topic>();
            Liked = new List<UserFavourite>();
            Bookmarked = new List<UserBookmark>();
            Tags = new List<Tag>();
        }
    }
}

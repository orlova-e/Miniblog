using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Comments")]
    public class Comment : Entity
    {
        public Guid AuthorId { get; set; }
        public User Author { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public DateTimeOffset? UpdatedDateTime { get; set; }
        public Guid? ParentId { get; set; }
        public Comment Parent { get; set; }
        public List<Comment> Children { get; set; }
        public List<CommentLikes> Likes { get; set; }
        public Comment()
        {
            Children = new List<Comment>();
        }
    }
}
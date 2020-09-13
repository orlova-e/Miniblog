using System;

namespace Miniblog.Models.Entities
{
    public class CommentLikes
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}

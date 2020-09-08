using Miniblog.Models.Entities;
using System.Collections.Generic;

namespace Miniblog.ViewModels
{
    public class ArticleViewModel
    {
        public Article Article { get; set; }
        public User CurrentUser { get; set; }
        public CommentViewModel CommentForm { get; set; }
        public AnonymousCommentViewModel AnonymousCommentForm { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}

using Miniblog.Models.Entities;

namespace Miniblog.ViewModels
{
    public class ArticleReadViewModel
    {
        public Article Article { get; set; }
        public User CurrentUser { get; set; }
        public CommentViewModel CommentForm { get; set; }
        public CommentAnonymousViewModel AnonymousCommentForm { get; set; }
    }
}

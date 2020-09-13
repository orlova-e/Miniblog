using Miniblog.Models.Entities;

namespace Miniblog.ViewModels
{
    public class ArticleFromListViewModel
    {
        public Article Article { get; set; }
        public int LikesCount { get; set; }
        public int BookmarksCount { get; set; }
        public int CommentsCount { get; set; }
    }
}

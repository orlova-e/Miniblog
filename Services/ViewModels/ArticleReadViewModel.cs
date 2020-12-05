using Domain.Entities;

namespace Miniblog.ViewModels
{
    public class ArticleReadViewModel
    {
        public Article Article { get; set; }
        public User User { get; set; }
    }
}

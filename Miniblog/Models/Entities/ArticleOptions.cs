using System;
using Miniblog.Models.Entities.Enums;
using Miniblog.Configuration;

namespace Miniblog.Models.Entities
{
    public class ArticleOptions : BaseArticlesOptions
    {
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public ColorTheme ColorTheme { get; set; }
        public static explicit operator ArticleOptions(ListOptions listOptions)
        {
            ArticleOptions articleOptions = new ArticleOptions()
            {
                Username = listOptions.Username,
                DateAndTime = listOptions.DateAndTime,
                Tags = listOptions.Tags,
                Topic = listOptions.Topic,
                Series = listOptions.Series,
                Likes = listOptions.Likes,
                Bookmarks = listOptions.Bookmarks,
                Comments = listOptions.Comments
            };
            return articleOptions;
        }
    }
}

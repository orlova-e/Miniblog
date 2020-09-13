using System;
using Miniblog.Models.Entities.Enums;

namespace Miniblog.Models.Entities
{
    public class ArticleOptions : BaseArticlesOptions
    {
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public ColorTheme ColorTheme { get; set; }
        public static explicit operator ArticleOptions(ListDisplayOptions globalOptions)
        {
            ArticleOptions articleOptions = new ArticleOptions()
            {
                Username = globalOptions.Username,
                //UserIcon = globalOptions.UserIcon,
                DateAndTime = globalOptions.DateAndTime,
                //Time = globalOptions.Time,
                Tags = globalOptions.Tags,
                Topic = globalOptions.Topic,
                Likes = globalOptions.Likes,
                Bookmarks = globalOptions.Bookmarks,
                Comments = globalOptions.Comments
            };
            return articleOptions;
        }
    }
}

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

        //public static explicit operator ListOptions(ArticleOptions articleOptions)
        //{
        //    ListOptions listOptions = new ListOptions()
        //    {
        //        Username = articleOptions.Username,
        //        DateAndTime = articleOptions.DateAndTime,
        //        Tags = articleOptions.Tags,
        //        Topic = articleOptions.Topic,
        //        Series = articleOptions.Series,
        //        Likes = articleOptions.Likes,
        //        Bookmarks = articleOptions.Bookmarks,
        //        Comments = articleOptions.Comments
        //    };
        //    return listOptions;
        //}

        //public static explicit operator ArticleOptions(ListDisplayOptions globalOptions)
        //{
        //    ArticleOptions articleOptions = new ArticleOptions()
        //    {
        //        Username = globalOptions.Username,
        //        DateAndTime = globalOptions.DateAndTime,
        //        Tags = globalOptions.Tags,
        //        Topic = globalOptions.Topic,
        //        Series = globalOptions.Series,
        //        Likes = globalOptions.Likes,
        //        Bookmarks = globalOptions.Bookmarks,
        //        Comments = globalOptions.Comments
        //    };
        //    return articleOptions;
        //}

        //public static explicit operator ListDisplayOptions(ArticleOptions articleOptions)
        //{
        //    ListDisplayOptions listDisplayOptions = new ListDisplayOptions()
        //    {
        //        Username = articleOptions.Username,
        //        DateAndTime = articleOptions.DateAndTime,
        //        Tags = articleOptions.Tags,
        //        Topic = articleOptions.Topic,
        //        Series = articleOptions.Series,
        //        Likes = articleOptions.Likes,
        //        Bookmarks = articleOptions.Bookmarks,
        //        Comments = articleOptions.Comments
        //    };
        //    return listDisplayOptions;
        //}
    }
}

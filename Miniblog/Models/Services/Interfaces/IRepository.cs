using Miniblog.Models.Entities;
using System;

namespace Miniblog.Models.Services.Interfaces
{
    /// <summary>
    /// Unit of work.
    /// </summary>
    public interface IRepository : IDisposable
    {
        IPlainRepository<User> Users { get; }
        IPlainRepository<Article> Articles { get; }
        IPlainRepository<Comment> Comments { get; }
        IPlainRepository<Topic> Topics { get; }
        IPlainRepository<Series> Series { get; }
        IOptionRepository<Role> Roles { get; }
        //IOptionRepository<ListDisplayOptions> ListDisplayOptions { get; }
        IOptionRepository<ArticleOptions> ArticleOptions { get; }
        //IOptionRepository<CommentsOptions> CommentsOptions { get; }
        //IOptionRepository<WebsiteOptions> WebsiteOptions { get; }
        IRelatedRepository<UserFavourite, Article> ArticleLikes { get; }
        IRelatedRepository<UserBookmark, Article> ArticleBookmarks { get; }
        IRelatedRepository<CommentLikes, Comment> CommentLikes { get; }
    }
}

using Domain.Entities;
using System;

namespace Repo.Interfaces
{
    /// <summary>
    /// Unit of work
    /// </summary>
    public interface IRepository : IDisposable
    {
        IPlainRepository<User> Users { get; }
        IPlainRepository<Article> Articles { get; }
        IPlainRepository<Comment> Comments { get; }
        IPlainRepository<Topic> Topics { get; }
        IPlainRepository<Series> Series { get; }
        IWorkingWithRange<FoundWord> FoundWords { get; }
        IWorkingWithRange<IndexInfo> IndexInfos { get; }
        IOptionRepository<Role> Roles { get; }
        IOptionRepository<ArticleOptions> ArticleOptions { get; }
        IOptionRepository<CheckList> CheckLists { get; }
        IRelatedRepository<UserFavourite, Article> ArticleLikes { get; }
        IRelatedRepository<UserBookmark, Article> ArticleBookmarks { get; }
        IRelatedRepository<CommentLikes, Comment> CommentLikes { get; }
        ISubscriptionsRepository Subscriptions { get; }
    }
}

using Domain.Entities;
using Repo.Interfaces;
using System;

namespace Repo.Implementation
{
    public class Repository : IRepository
    {
        public MiniblogDb Db { get; private set; }

        IPlainRepository<User> users;
        IPlainRepository<Article> articles;
        IPlainRepository<Comment> comments;
        IPlainRepository<Topic> topics;
        IPlainRepository<Series> series;

        IWorkingWithRange<FoundWord> foundWords;
        IWorkingWithRange<IndexInfo> indexInfos;

        IOptionRepository<Role> roles;
        IOptionRepository<ArticleOptions> articleOptions;

        IRelatedRepository<UserFavourite, Article> articleLikes;
        IRelatedRepository<UserBookmark, Article> articleBookmarks;
        IRelatedRepository<CommentLikes, Comment> commentLikes;

        ISubscriptionsRepository subscriptions;

        public Repository(MiniblogDb miniblogDb)
        {
            Db = miniblogDb;
        }
        public IPlainRepository<User> Users
        {
            get
            {
                if (users == null)
                    users = new UsersRepository(Db);
                return users;
            }
        }
        public IPlainRepository<Article> Articles
        {
            get
            {
                if (articles == null)
                    articles = new ArticlesRepository(Db);
                return articles;
            }
        }
        public IPlainRepository<Comment> Comments
        {
            get
            {
                if (comments == null)
                    comments = new CommentsRepository(Db);
                return comments;
            }
        }
        public IPlainRepository<Topic> Topics
        {
            get
            {
                if (topics == null)
                    topics = new TopicsRepository(Db);
                return topics;
            }
        }
        public IPlainRepository<Series> Series
        {
            get
            {
                if (series == null)
                    series = new SeriesRepository(Db);
                return series;
            }
        }
        public IWorkingWithRange<FoundWord> FoundWords
        {
            get
            {
                if (foundWords == null)
                    foundWords = new FoundWordsRepository(Db);
                return foundWords;
            }
        }
        public IWorkingWithRange<IndexInfo> IndexInfos
        {
            get
            {
                if (indexInfos == null)
                    indexInfos = new IndexInfoRepository(Db);
                return indexInfos;
            }
        }
        public IOptionRepository<Role> Roles
        {
            get
            {
                if (roles == null)
                    roles = new RolesRepository(Db);
                return roles;
            }
        }
        public IOptionRepository<ArticleOptions> ArticleOptions
        {
            get
            {
                if (articleOptions == null)
                    articleOptions = new ArticleOptionsRepo(Db);
                return articleOptions;
            }
        }
        public IRelatedRepository<UserFavourite, Article> ArticleLikes
        {
            get
            {
                if (articleLikes == null)
                    articleLikes = new ArticleLikesRepo(Db);
                return articleLikes;
            }
        }
        public IRelatedRepository<UserBookmark, Article> ArticleBookmarks
        {
            get
            {
                if (articleBookmarks == null)
                    articleBookmarks = new ArticleBookmarksRepo(Db);
                return articleBookmarks;
            }
        }
        public IRelatedRepository<CommentLikes, Comment> CommentLikes
        {
            get
            {
                if (commentLikes == null)
                    commentLikes = new CommentLikeRepo(Db);
                return commentLikes;
            }
        }

        public ISubscriptionsRepository Subscriptions
        {
            get
            {
                if (subscriptions == null)
                    subscriptions = new SubscriptionsRepository(this);
                return subscriptions;
            }
        }

        private bool disposed = false;
        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
                Db.Dispose();
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

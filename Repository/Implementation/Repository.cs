using Domain.Entities;
using Repo.Interfaces;
using System;

namespace Repo.Implementation
{
    public class Repository : IRepository
    {
        public MiniblogDb Db { get; private set; }

        IPlainRepository<Entity> entities;
        IPlainRepository<User> users;
        IPlainRepository<Article> articles;
        IPlainRepository<Comment> comments;
        IPlainRepository<Topic> topics;
        IPlainRepository<Series> series;

        IPlainEntityRepository<Tag> tags;
        IPlainEntityRepository<FoundWord> foundWords;
        IPlainEntityRepository<IndexInfo> indexInfos;

        IOptionRepository<Role> roles;
        IOptionRepository<ArticleOptions> articleOptions;
        IOptionRepository<CheckList> checkLists;

        IRelatedRepository<UserFavourite, Article> articleLikes;
        IRelatedRepository<UserBookmark, Article> articleBookmarks;
        IRelatedRepository<CommentLikes, Comment> commentLikes;

        ISubscriptionsRepository subscriptions;

        public Repository(MiniblogDb miniblogDb)
        {
            Db = miniblogDb;
        }
        public IPlainRepository<Entity> Entities
        {
            get
            {
                if (entities == null)
                    entities = new EntityRepository(Db);
                return entities;
            }
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
        public IPlainEntityRepository<Tag> Tags
        {
            get
            {
                if (tags == null)
                    tags = new TagsRepository(Db);
                return tags;
            }
        }
        public IPlainEntityRepository<FoundWord> FoundWords
        {
            get
            {
                if (foundWords == null)
                    foundWords = new FoundWordsRepository(Db);
                return foundWords;
            }
        }
        public IPlainEntityRepository<IndexInfo> IndexInfos
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

        public IOptionRepository<CheckList> CheckLists
        {
            get
            {
                if (checkLists == null)
                    checkLists = new CheckListsRepository(Db);
                return checkLists;
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

using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class Repository : IRepository
    {
        public MiniblogDb Db { get; private set; }
        private IPlainRepository<User> users;
        private IPlainRepository<Article> articles;
        private IPlainRepository<Comment> comments;
        private IPlainRepository<Topic> topics;
        private IOptionRepository<Role> roles;
        //private IOptionRepository<Opportunities> opportunities;
        private IOptionRepository<ListDisplayOptions> listDisplayOptions;
        private IOptionRepository<ArticleOptions> articleOptions;
        private IOptionRepository<WebsiteOptions> websiteOptions;
        private IRelatedRepository<UserFavourite, Article> articleLikes;
        private IRelatedRepository<UserBookmark, Article> articleBookmarks;
        private IRelatedRepository<CommentLikes, Comment> commentLikes;
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
        public IOptionRepository<Role> Roles
        {
            get
            {
                if (roles == null)
                    roles = new RolesRepository(Db);
                return roles;
            }
        }
        //public IOptionRepository<Opportunities> Opportunities
        //{
        //    get
        //    {
        //        if (opportunities == null)
        //            opportunities = new OpportinitiesRepository(Db);
        //        return opportunities;
        //    }
        //}
        public IOptionRepository<ListDisplayOptions> ListDisplayOptions
        {
            get
            {
                if (listDisplayOptions == null)
                    listDisplayOptions = new ListOptionsRepo(Db);
                return listDisplayOptions;
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
        public IOptionRepository<WebsiteOptions> WebsiteOptions
        {
            get
            {
                if (websiteOptions == null)
                    websiteOptions = new WebsiteOptionsRepo(Db);
                return websiteOptions;
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

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
        public MiniblogDb Db { get; }
        private IPlainRepository<User> users;
        private IPlainRepository<RefreshToken> refreshTokens;
        private IPlainRepository<Article> articles;
        private IPlainRepository<Comment> comments;
        private IPlainRepository<Topic> topics;
        private IOptionRepository<Role> roles;
        //private IOptionRepository<Opportunities> opportunities;
        private IOptionRepository<ListDisplayOptions> listDisplayOptions;
        private IOptionRepository<ArticleOptions> articleOptions;
        private IOptionRepository<WebsiteOptions> websiteOptions;
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
        public IPlainRepository<RefreshToken> RefreshTokens
        {
            get
            {
                if (refreshTokens == null)
                    refreshTokens = new RefreshTokenRepository(Db);
                return refreshTokens;
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

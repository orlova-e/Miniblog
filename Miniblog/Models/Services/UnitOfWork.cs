using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        public MiniblogDb Db { get; }
        private IPlainRepository<User> users;
        private IPlainRepository<Article> articles;
        private IPlainRepository<Comment> comments;
        private IPlainRepository<Topic> topics;
        private IOptionRepository<Role> roles;
        //private IOptionRepository<Opportunities> opportunities;
        private IOptionRepository<ArticlesListDisplayOptions> articlesListDisplay;
        private IOptionRepository<UserArticleDisplayOptions> userArticleDisplayOptions;
        private IOptionRepository<WebsiteDisplayOptions> websiteDisplayOptions;
        public UnitOfWork(MiniblogDb miniblogDb)
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
        public IOptionRepository<ArticlesListDisplayOptions> ArticlesListOptions
        {
            get
            {
                if (articlesListDisplay == null)
                    articlesListDisplay = new ArticlesListOptionsRepo(Db);
                return articlesListDisplay;
            }
        }
        public IOptionRepository<UserArticleDisplayOptions> UserArticleOptions
        {
            get
            {
                if (userArticleDisplayOptions == null)
                    userArticleDisplayOptions = new UserArticleOptionsRepo(Db);
                return userArticleDisplayOptions;
            }
        }
        public IOptionRepository<WebsiteDisplayOptions> WebsiteDisplayOptions
        {
            get
            {
                if (websiteDisplayOptions == null)
                    websiteDisplayOptions = new WebsiteOptionsRepo(Db);
                return websiteDisplayOptions;
            }
        }
    }
}

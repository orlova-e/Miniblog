using Miniblog.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    public interface IUnitOfWork
    {
        IPlainRepository<User> Users { get; }
        IPlainRepository<Article> Articles { get; }
        IPlainRepository<Comment> Comments { get; }
        IPlainRepository<Topic> Topics { get; }
        //IPlainRepository<Series> Series { get; }

        IOptionRepository<Role> Roles { get; }
        //IOptionRepository<Opportunities> Opportunities { get; }
        IOptionRepository<ArticlesListDisplayOptions> ArticlesListOptions { get; }
        IOptionRepository<UserArticleDisplayOptions> UserArticleOptions { get; }
        IOptionRepository<WebsiteDisplayOptions> WebsiteDisplayOptions { get; }
    }
}

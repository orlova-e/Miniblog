using Miniblog.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    /// <summary>
    /// UnitOfWork pattern.
    /// </summary>
    public interface IRepository : IDisposable
    {
        IPlainRepository<User> Users { get; }
        IPlainRepository<Article> Articles { get; }
        IPlainRepository<Comment> Comments { get; }
        IPlainRepository<Topic> Topics { get; }
        //IPlainRepository<Series> Series { get; }

        IOptionRepository<Role> Roles { get; }
        //IOptionRepository<Opportunities> Opportunities { get; }
        IOptionRepository<ListDisplayOptions> ListDisplayOptions { get; }
        IOptionRepository<ArticleOptions> ArticleOptions { get; }
        IOptionRepository<WebsiteOptions> WebsiteOptions { get; }
    }
}

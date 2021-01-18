using Domain.Entities;
using Repo.Interfaces;
using Services.Interfaces.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation.Search
{
    public class ArticleSearchStrategy : ISearchStrategy<Article>
    {
        public List<Func<Article, bool>> Predicates { get; }
        //public Func<Func<Article, bool>, IEnumerable<Article>> Find { get; }
        public Func<Func<Article, bool>, Task<IEnumerable<Article>>> FindAsync { get; }
        //public Func<Guid, Task<IEnumerable<Article>>> GetByIdAsync { get; }
        //public Func<IEnumerable<Guid>, Task<IEnumerable<Article>>> GetRangeAsync { get; }

        public ArticleSearchStrategy(IRepository repository,
            string query)
        {
            //Find = repository.Articles.Find;
            FindAsync = repository.Articles.FindAsync;
            Predicates = new List<Func<Article, bool>>();
            Predicates.Add(u => u.Header.Contains(query));
        }
    }
}

using Domain.Entities;
using Repo.Interfaces;
using Services.FoundValues;
using Services.Interfaces.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation.Search
{
    public class AccurateSearch<T> : ISearch<T>
        where T : class, new()
    {
        private IRepository Repository { get; }
        public AccurateSearch(IRepository repository)
        {
            Repository = repository;
        }

        public async Task<List<FoundObject<T>>> FindAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException();

            List<FoundObject<T>> foundObjects = new List<FoundObject<T>>();
            List<T> foundEntities = new List<T>();

            ISearchStrategy<T> searchStrategy = ChooseStrategy(query);

            foreach (var predicate in searchStrategy.Predicates)
            {
                IEnumerable<T> list = await searchStrategy.FindAsync.Invoke(predicate);
                if (list is object)
                    foundEntities = foundEntities.Union(list).ToList();
            }

            foundObjects = (from found in foundEntities
                            select new FoundObject<T>
                            {
                                Entity = found,
                                MatchedWords = new List<string> { query },
                                TotalRating = int.MaxValue
                            }).ToList();

            return foundObjects;
        }

        protected ISearchStrategy<T> ChooseStrategy(string query)
        {
            Type type = typeof(T);

            if (type == typeof(User))
            {
                return new UserSearchStrategy(Repository, query) as ISearchStrategy<T>;
            }
            else if (type == typeof(Article))
            {
                return new ArticleSearchStrategy(Repository, query) as ISearchStrategy<T>;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}

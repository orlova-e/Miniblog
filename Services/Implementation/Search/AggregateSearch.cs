using Domain.Entities;
using Repo.Interfaces;
using Services.FoundValues;
using Services.Interfaces.Search;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation.Search
{
    /// <summary>
    /// Searches for an entity T using accurate search and indexed word search
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    public class AggregateSearch<T> : IAggregateSearch<T>
        where T : Entity, new()
    {
        protected virtual ISearch<T> AccurateSearch { get; set; }
        protected virtual ISearch<T> SearchByWords { get; set; }
        private IRepository Repository { get; }
        public AggregateSearch(IRepository repository)
        {
            Repository = repository;
            AccurateSearch = new AccurateSearch<T>(Repository);
            SearchByWords = new SearchByWords<T>(Repository);
        }

        public async Task<List<FoundObject>> FindAsync(string query, bool accurateSearch = false)
        {
            List<FoundObject> foundAccurately, foundByWords = null;

            foundAccurately = await AccurateSearch.FindAsync(query);

            if (!accurateSearch)
            {
                foundByWords = await SearchByWords.FindAsync(query);
            }

            List<FoundObject> foundObjects = PrepareResults(foundAccurately, foundByWords);

            return foundObjects;
        }

        protected List<FoundObject> PrepareResults(List<FoundObject> foundAccurately,
            List<FoundObject> foundByWords = null)
        {
            List<FoundObject> result = new List<FoundObject>();

            if (foundByWords?.Any() ?? default)
            {
                var joinedResult = from accurately in foundAccurately
                                   join byWords in foundByWords on accurately.Entity.Id equals byWords.Entity.Id
                                   select new FoundObject
                                   {
                                       Entity = accurately.Entity,
                                       MatchedWords = accurately.MatchedWords.Concat(byWords.MatchedWords).Distinct().ToList(),
                                       TotalRating = accurately.TotalRating > int.MaxValue - byWords.TotalRating
                                                     ? int.MaxValue : accurately.TotalRating + byWords.TotalRating
                                   };

                foundAccurately = foundAccurately.ExceptBy(f => f.Entity.Id, joinedResult).ToList();
                foundByWords = foundByWords.ExceptBy(f => f.Entity.Id, joinedResult).ToList();

                result.AddRange(joinedResult);
                result.AddRange(foundAccurately);
                result.AddRange(foundByWords);
            }
            else
            {
                result = foundAccurately;
            }

            result = result
                .OrderByDescending(f => f.TotalRating)
                .ThenByDescending(f => f.MatchedWords.Count)
                .ToList();

            return result.ToList();
        }
    }
}

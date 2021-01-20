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
        where T : class, new()
    {
        public ISearch<T> Search { get; protected set; }
        private IRepository Repository { get; }
        public AggregateSearch(IRepository repository)
        {
            Repository = repository;
        }

        public async Task<List<FoundObject<T>>> FindAsync(string query, bool accurateSearch = false)
        {
            List<FoundObject<T>> foundAccurately, foundByWords = null;

            Search = new AccurateSearch<T>(Repository);
            foundAccurately = await Search.FindAsync(query);

            if (!accurateSearch)
            {
                Search = new SearchByWords<T>(Repository);
                foundByWords = await Search.FindAsync(query);
            }

            List<FoundObject<T>> foundObjects = PrepareResults(foundAccurately, foundByWords);

            return foundObjects;
        }

        protected List<FoundObject<T>> PrepareResults(List<FoundObject<T>> foundAccurately,
            List<FoundObject<T>> foundByWords = null)
        {
            List<FoundObject<T>> result = new List<FoundObject<T>>();

            if (foundByWords is object)
            {
                var joinedResult = from accurately in foundAccurately
                                   join byWords in foundByWords on accurately.EntityId equals byWords.EntityId
                                   select new FoundObject<T>
                                   {
                                       EntityId = accurately.EntityId,
                                       Entity = accurately.Entity,
                                       MatchedWords = accurately.MatchedWords.Concat(byWords.MatchedWords).ToList(),
                                       TotalRating = accurately.TotalRating > int.MaxValue - byWords.TotalRating
                                                     ? int.MaxValue : accurately.TotalRating + byWords.TotalRating
                                   };

                foundAccurately = (from accurately in foundAccurately
                                   from joined in joinedResult
                                   where accurately.EntityId != joined.EntityId
                                   select accurately).ToList();

                foundByWords = (from byWords in foundByWords
                                from joined in joinedResult
                                where byWords.EntityId != joined.EntityId
                                select byWords).ToList();

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

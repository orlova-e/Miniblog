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

        public async Task<List<FoundObject<T>>> FindAsync(string query, bool accurateSearch = false)
        {
            List<FoundObject<T>> foundAccurately, foundByWords = null;

            foundAccurately = await AccurateSearch.FindAsync(query);

            if (!accurateSearch)
            {
                foundByWords = await SearchByWords.FindAsync(query);
            }

            List<FoundObject<T>> foundObjects = PrepareResults(foundAccurately, foundByWords);

            return foundObjects;
        }

        protected List<FoundObject<T>> PrepareResults(List<FoundObject<T>> foundAccurately,
            List<FoundObject<T>> foundByWords = null)
        {
            List<FoundObject<T>> result = new List<FoundObject<T>>();

            if (foundByWords?.Any() ?? default)
            {
                var joinedResult = from accurately in foundAccurately
                                   join byWords in foundByWords on accurately.Entity.Id equals byWords.Entity.Id
                                   select new FoundObject<T>
                                   {
                                       //EntityId = accurately.Entity.Id,
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

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
    /// Searches for entities by words and ranks them according to the match
    /// </summary>
    /// <typeparam name="T">Type of entity being searched for</typeparam>
    public class SearchByWords<T> : ISearch<T>
        where T : class, new()
    {
        private IRepository Repository { get; }
        public SearchByWords(IRepository repository)
        {
            Repository = repository;
        }

        public async Task<List<FoundObject<T>>> FindAsync(string query)
        {
            List<string> preparedQuery = InputPreparation.Prepare(query);
            List<FoundWord> foundWords = await FindWordsAsync(preparedQuery);

            List<FoundObject<T>> preparedResults = new List<FoundObject<T>>();
            if (foundWords.Any())
            {
                preparedResults = PrepareResults(foundWords);
            }

            return preparedResults;
        }

        protected List<FoundObject<T>> PrepareResults(List<FoundWord> foundWords)
        {
            IEnumerable<Guid> ids = foundWords
                .SelectMany(f => f.IndexInfos)
                .Select(i => i.EntityId)
                .Distinct();

            List<FoundObject<T>> foundObjects = new List<FoundObject<T>>(ids.Count());

            foreach (Guid id in ids)
            {
                FoundObject<T> foundObject = new FoundObject<T>
                {
                    EntityId = id,

                    Entity = foundWords
                        .SelectMany(f => f.IndexInfos)
                        .Where(i => i.EntityId == id)
                        .Where(i => i.Entity is T)
                        .Select(i => i.Entity)
                        .First() as T,

                    MatchedWords = foundWords
                        .SelectMany(f => f.IndexInfos)
                        .Where(i => i.EntityId == id)
                        .Select(i => i.FoundWord.Word)
                        .Distinct()
                        .ToList(),

                    TotalRating = foundWords
                        .SelectMany(f => f.IndexInfos)
                        .Where(i => i.EntityId == id)
                        .Select(i => i.Rank)
                        .Sum()
                };

                foundObjects.Add(foundObject);
            }

            foundObjects = foundObjects
                .OrderByDescending(f => f.TotalRating)
                .ThenByDescending(f => f.MatchedWords.Count)
                .ToList();

            return foundObjects;
        }

        protected virtual async Task<List<FoundWord>> FindWordsAsync(List<string> words)
        {
            List<FoundWord> foundWords = new List<FoundWord>();
            foreach (string word in words)
            {
                var fromRepo = await Repository.FoundWords
                                .FindAsync(fw => fw.Word.Equals(word, StringComparison.OrdinalIgnoreCase));

                if (fromRepo == null)
                    continue;

                var found = from foundword in fromRepo
                            from indexInfo in foundword.IndexInfos
                            where indexInfo.Entity is T
                            select foundword;

                foundWords.AddRange(found);
            }
            return foundWords;
        }
    }
}

using Domain.Entities;
using Repo.Interfaces;
using Services.IndexedValues;
using Services.Interfaces.Indexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation.Indexing
{
    public class IndexedObjectsObserver : IIndexedObjectsObserver
    {
        private IRepository Repository { get; }
        public IndexedObjectsObserver(IRepository repository)
        {
            Repository = repository;
        }

        protected IRateStrategy ChooseStrategy(IndexedObject indexedObject)
        {
            if (indexedObject is ArticleIndexedValues)
                return new ArticleRateStrategy();
            else if (indexedObject is UserIndexedValues)
                return new UserRateStrategy();
            else
                throw new ArgumentException();
        }

        public async Task OnNewEntityAsync(IndexedObject indexedObject)
        {
            IRateStrategy rateStrategy = ChooseStrategy(indexedObject);
            IndexObject indexObject = new IndexObject(Repository, rateStrategy);
            List<FoundWord> foundWords = indexObject.Index(indexedObject);
            foreach (var foundword in foundWords)
            {
                if (Repository.FoundWords.Find(fw => fw.Id == foundword.Id).Any())
                {
                    await Repository.FoundWords.UpdateAsync(foundword);
                }
                else
                {
                    await Repository.FoundWords.CreateAsync(foundword);
                }
            }
        }

        public async Task OnUpdatedEntityAsync(IndexedObject indexedObject)
        {
            List<FoundWord> oldFoundWords = Repository.IndexInfos
                .Find(ii => ii.EntityId == indexedObject.Id)
                .Select(ii => ii.FoundWord)
                .ToList();

            IRateStrategy rateStrategy = ChooseStrategy(indexedObject);
            IndexObject indexObject = new IndexObject(Repository, rateStrategy);
            List<FoundWord> foundWords = indexObject.Index(indexedObject);

            var newFoundWords = from foundWord in foundWords
                                from oldFoundWord in oldFoundWords
                                where foundWord.Word != oldFoundWord.Word
                                select foundWord;

            var updatedFoundWords = from foundWord in foundWords
                                    from oldFoundWord in oldFoundWords
                                    where foundWord.Word == oldFoundWord.Word
                                    select foundWord;

            if (newFoundWords.Any())
                await Repository.FoundWords.CreateRangeAsync(newFoundWords);
            if (updatedFoundWords.Any())
                await Repository.FoundWords.UpdateRangeAsync(updatedFoundWords);

            var excludedFoundWords = from oldFoundWord in oldFoundWords
                                     from foundWord in foundWords
                                     where oldFoundWord.Word != foundWord.Word
                                     select oldFoundWord;

            var excludedIndexInfos = from foundWord in excludedFoundWords
                                     from indexInfo in foundWord.IndexInfos
                                     where indexInfo.EntityId != indexedObject.Id
                                     select indexInfo;

            if (excludedIndexInfos.Any())
            {
                excludedIndexInfos = from foundWord in excludedFoundWords
                                     from indexInfo in foundWord.IndexInfos
                                     where indexInfo.EntityId == indexedObject.Id
                                     select indexInfo;
                if (excludedIndexInfos.Any())
                    await Repository.IndexInfos.DeleteRangeAsync(excludedIndexInfos);
            }
            else if (excludedFoundWords.Any())
            {
                await Repository.FoundWords.DeleteRangeAsync(excludedFoundWords);
            }
        }

        public async Task OnDeletedEntityAsync(IndexedObject indexedObject)
        {
            List<FoundWord> foundWords = Repository.IndexInfos
                .Find(ii => ii.EntityId == indexedObject.Id)
                .Select(ii => ii.FoundWord)
                .Distinct()
                .ToList();

            var leftWords = (from foundWord in foundWords
                             from indexInfo in foundWord.IndexInfos
                             where indexInfo.EntityId != indexedObject.Id
                             select foundWord)?
                            .Distinct();

            var foundWordsToDelete = from foundWord in foundWords.Except(leftWords)
                                     select foundWord;
            if (foundWordsToDelete.Any())
                await Repository.FoundWords.DeleteRangeAsync(foundWordsToDelete);

            var indexInfosToDelete = from foundWord in leftWords
                                     from indexInfo in foundWord.IndexInfos
                                     where indexInfo.EntityId == indexedObject.Id
                                     select indexInfo;
            if (indexInfosToDelete.Any())
                await Repository.IndexInfos.DeleteRangeAsync(indexInfosToDelete);
        }
    }
}

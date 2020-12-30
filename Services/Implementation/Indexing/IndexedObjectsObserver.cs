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
                if (Repository.FoundWords.Find(f => f.Id == foundword.Id).Any())
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

            List<FoundWord> newFoundWords = foundWords.Except(oldFoundWords).ToList();
            List<FoundWord> updatedFoundWords = foundWords.Except(newFoundWords).ToList();
            List<FoundWord> excludedFoundWords = oldFoundWords.Except(foundWords).ToList();

            await Repository.FoundWords.CreateRangeAsync(newFoundWords);
            await Repository.FoundWords.UpdateRangeAsync(updatedFoundWords);

            foreach (var excepted in excludedFoundWords)
            {
                var deletedIndexInfos = excepted.IndexInfos.Where(ii => ii.EntityId == indexedObject.Id);
                if (deletedIndexInfos.Any())
                {
                    await Repository.IndexInfos.DeleteRangeAsync(deletedIndexInfos);
                }
            }

            var deletedFoundWords = from excluded in excludedFoundWords
                                    from indexInfo in excluded.IndexInfos
                                    where indexInfo.EntityId == indexedObject.Id
                                    select excluded;
            await Repository.FoundWords.DeleteRangeAsync(deletedFoundWords);
        }

        public async Task OnDeletedEntityAsync(IndexedObject indexedObject)
        {
            List<FoundWord> foundWords = Repository.IndexInfos
                .Find(ii => ii.EntityId == indexedObject.Id)
                .Select(ii => ii.FoundWord)
                .ToList();

            var leftWords = from foundWord in foundWords
                            from indexInfo in foundWord.IndexInfos
                            where indexInfo.EntityId != indexedObject.Id
                            select foundWord;
            var foundWordsToDelete = from foundWord in foundWords.Except(leftWords)
                                     select foundWord;
            await Repository.FoundWords.DeleteRangeAsync(foundWordsToDelete);

            var indexInfosToDelete = from foundWord in leftWords
                                     from indexInfo in foundWord.IndexInfos
                                     where indexInfo.EntityId == indexedObject.Id
                                     select indexInfo;
            await Repository.IndexInfos.DeleteRangeAsync(indexInfosToDelete);
        }
    }
}

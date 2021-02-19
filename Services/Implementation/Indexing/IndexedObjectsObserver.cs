using Domain.Entities;
using Repo.Interfaces;
using Services.VisibleValues;
using Services.Interfaces.Indexing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation.Indexing
{
    public class IndexedObjectsObserver : IVisibleObjectsObserver
    {
        private IRepository Repository { get; }
        public IndexedObjectsObserver(IRepository repository)
        {
            Repository = repository;
        }

        public async Task CheckNewEntityAsync(VisibleObjectValues visibleValues)
        {
            IndexObject indexObject = new IndexObject(Repository);
            List<FoundWord> foundWords = indexObject.Index(visibleValues);
            foreach (var foundword in foundWords)
            {
                if (Repository.FoundWords.Find(fw => fw.Word == foundword.Word).Any())
                {
                    await Repository.FoundWords.UpdateAsync(foundword);
                }
                else
                {
                    await Repository.FoundWords.CreateAsync(foundword);
                }
            }
        }

        public async Task CheckUpdatedEntityAsync(VisibleObjectValues visibleValues)
        {
            List<FoundWord> oldFoundWords = Repository.IndexInfos
                .Find(ii => ii.EntityId == visibleValues.Id)
                .Select(ii => ii.FoundWord)
                .ToList();

            IndexObject indexObject = new IndexObject(Repository);
            List<FoundWord> foundWords = indexObject.Index(visibleValues);

            var newFoundWords = foundWords.ExceptBy(f => f.Word, oldFoundWords);

            var updatedFoundWords = foundWords.IntersectBy(f => f.Word, oldFoundWords);

            if (newFoundWords.Any())
                await Repository.FoundWords.CreateRangeAsync(newFoundWords);

            if (updatedFoundWords.Any())
                await Repository.FoundWords.UpdateRangeAsync(updatedFoundWords);

            var excludedFoundWords = oldFoundWords.ExceptBy(f => f.Word, foundWords);

            var excludedIndexInfos = excludedFoundWords
                .SelectMany(f => f.IndexInfos)
                .Where(i => i.EntityId != visibleValues.Id);

            if (excludedIndexInfos.Any())
            {
                excludedIndexInfos = excludedFoundWords
                    .SelectMany(f => f.IndexInfos)
                    .Where(i => i.EntityId == visibleValues.Id);

                if (excludedIndexInfos.Any())
                    await Repository.IndexInfos.DeleteRangeAsync(excludedIndexInfos);
            }
            else if (excludedFoundWords.Any())
            {
                await Repository.FoundWords.DeleteRangeAsync(excludedFoundWords);
            }
        }

        public async Task CheckDeletedEntityAsync(VisibleObjectValues visibleValues)
        {
            List<FoundWord> foundWords = Repository.IndexInfos
                .Find(ii => ii.EntityId == visibleValues.Id)
                .Select(ii => ii.FoundWord)
                .Distinct()
                .ToList();

            var leftWords = (from foundWord in foundWords
                             from indexInfo in foundWord.IndexInfos
                             where indexInfo.EntityId != visibleValues.Id
                             select foundWord)?
                            .Distinct();

            var foundWordsToDelete = foundWords.ExceptBy(f => f.Word, leftWords);

            if (foundWordsToDelete.Any())
                await Repository.FoundWords.DeleteRangeAsync(foundWordsToDelete);

            var indexInfosToDelete = leftWords
                .SelectMany(f => f.IndexInfos)
                .Where(i => i.EntityId == visibleValues.Id);

            if (indexInfosToDelete.Any())
                await Repository.IndexInfos.DeleteRangeAsync(indexInfosToDelete);
        }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class FoundWordsRepository : IPlainEntityRepository<FoundWord>
    {
        public MiniblogDb Db { get; private set; }
        public FoundWordsRepository(MiniblogDb db)
        {
            Db = db;
        }

        public async Task<FoundWord> GetByIdAsync(Guid id)
        {
            FoundWord foundWord = await Db.FoundWords.FindAsync(id);
            foreach (var indexInfo in foundWord.IndexInfos)
            {
                Type entityType = DeterminingType.Determine(indexInfo.EntityType);
                indexInfo.Entity = await Db.FindAsync(entityType, indexInfo.EntityId);
            }
            return foundWord;
        }

        public async Task<IEnumerable<FoundWord>> GetAllAsync()
        {
            IEnumerable<FoundWord> foundWords = await Db.FoundWords.ToListAsync();

            foreach (var foundWord in foundWords)
                foreach (var indexInfo in foundWord.IndexInfos)
                {
                    Type entityType = DeterminingType.Determine(indexInfo.EntityType);
                    indexInfo.Entity = await Db.FindAsync(entityType, indexInfo.EntityId);
                }

            return foundWords;
        }

        public IEnumerable<FoundWord> Find(Func<FoundWord, bool> predicate)
        {
            IEnumerable<FoundWord> foundWords = Db.FoundWords.Where(predicate);

            foreach (var foundWord in foundWords)
                foreach (var indexInfo in foundWord.IndexInfos)
                {
                    Type entityType = DeterminingType.Determine(indexInfo.EntityType);
                    indexInfo.Entity = Db.Find(entityType, indexInfo.EntityId);
                }

            return foundWords;
        }

        public async Task<IEnumerable<FoundWord>> FindAsync(Func<FoundWord, bool> predicate)
        {
            IEnumerable<FoundWord> foundWords = await Task.Run(() => Db.FoundWords.Where(predicate));

            foreach (var foundWord in foundWords)
                foreach (var indexInfo in foundWord.IndexInfos)
                {
                    Type entityType = DeterminingType.Determine(indexInfo.EntityType);
                    indexInfo.Entity = await Db.FindAsync(entityType, indexInfo.EntityId);
                }

            return foundWords;
        }

        public async Task CreateAsync(FoundWord entity)
        {
            Db.FoundWords.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateAsync(FoundWord entity)
        {
            Db.FoundWords.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            FoundWord wordIndex = await Db.FoundWords.FindAsync(id);
            Db.FoundWords.Remove(wordIndex);
            await Db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<FoundWord> entities)
        {
            Db.FoundWords.AddRange(entities);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<FoundWord> entities)
        {
            Db.FoundWords.UpdateRange(entities);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<FoundWord> entities)
        {
            Db.FoundWords.RemoveRange(entities);
            await Db.SaveChangesAsync();
        }
    }
}

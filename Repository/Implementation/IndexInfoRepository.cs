using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class IndexInfoRepository : IWorkingWithRange<IndexInfo>
    {
        public MiniblogDb Db { get; set; }
        public IndexInfoRepository(MiniblogDb db)
        {
            Db = db;
        }

        public async Task<IndexInfo> GetByIdAsync(Guid id)
        {
            IndexInfo indexInfo = await Db.IndexInfos.FindAsync(id);
            Type entityType = DeterminingType.Determine(indexInfo.EntityType);
            indexInfo.Entity = await Db.FindAsync(entityType, indexInfo.EntityId);
            return indexInfo;
        }

        public async Task<IEnumerable<IndexInfo>> GetAllAsync()
        {
            List<IndexInfo> indexInfos = await Db.IndexInfos.ToListAsync();
            foreach (var indexInfo in indexInfos)
            {
                Type entityType = DeterminingType.Determine(indexInfo.EntityType);
                indexInfo.Entity = await Db.FindAsync(entityType, indexInfo.EntityId);
            }
            return indexInfos;
        }

        public IEnumerable<IndexInfo> Find(Func<IndexInfo, bool> predicate)
        {
            IEnumerable<IndexInfo> indexInfos = Db.IndexInfos.Include(i => i.FoundWord).Where(predicate);
            foreach (var indexinfo in indexInfos)
            {
                Type entityType = DeterminingType.Determine(indexinfo.EntityType);
                indexinfo.Entity = Db.Find(entityType, indexinfo.EntityId);
            }
            return indexInfos;
        }

        public async Task<IEnumerable<IndexInfo>> FindAsync(Func<IndexInfo, bool> predicate)
        {
            IEnumerable<IndexInfo> indexInfos = await Task.Run(() => Db.IndexInfos.Where(predicate));
            foreach (var indexinfo in indexInfos)
            {
                Type entityType = DeterminingType.Determine(indexinfo.EntityType);
                indexinfo.Entity = await Db.FindAsync(entityType, indexinfo.EntityId);
            }
            return indexInfos;
        }

        public async Task CreateAsync(IndexInfo entity)
        {
            Db.IndexInfos.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateAsync(IndexInfo entity)
        {
            Db.IndexInfos.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            IndexInfo indexInfo = await Db.IndexInfos.FindAsync(id);
            Db.IndexInfos.Remove(indexInfo);
            await Db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<IndexInfo> entities)
        {
            Db.IndexInfos.AddRange(entities);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<IndexInfo> entities)
        {
            Db.IndexInfos.UpdateRange(entities);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<IndexInfo> entities)
        {
            Db.IndexInfos.RemoveRange(entities);
            await Db.SaveChangesAsync();
        }
    }
}

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
            return await Db.IndexInfos.FindAsync(id);
        }

        public async Task<IEnumerable<IndexInfo>> GetAllAsync()
        {
            return await Db.IndexInfos.ToListAsync();
        }

        public IEnumerable<IndexInfo> Find(Func<IndexInfo, bool> predicate)
        {
            return Db.IndexInfos.Where(predicate);
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

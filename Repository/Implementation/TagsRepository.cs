using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class TagsRepository : IPlainEntityRepository<Tag>
    {
        public MiniblogDb Db { get; }
        public TagsRepository(MiniblogDb db)
        {
            Db = db;
        }

        public async Task<Tag> GetByIdAsync(Guid id)
        {
            return await Db.Tags.FindAsync(id);
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await Db.Tags.ToListAsync();
        }

        public IEnumerable<Tag> Find(Func<Tag, bool> predicate)
        {
            return Db.Tags.Where(predicate);
        }

        public async Task<IEnumerable<Tag>> FindAsync(Func<Tag, bool> predicate)
        {
            return await Task.Run(() => Find(predicate));
        }

        public async Task CreateAsync(Tag entity)
        {
            Db.Tags.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tag entity)
        {
            Db.Tags.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            Tag tag = await Db.Tags.FindAsync(id);
            Db.Tags.Remove(tag);
            await Db.SaveChangesAsync();
        }

        public async Task CreateRangeAsync(IEnumerable<Tag> entities)
        {
            await Db.AddRangeAsync(entities);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Tag> entities)
        {
            Db.UpdateRange(entities);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<Tag> entities)
        {
            Db.RemoveRange(entities);
            await Db.SaveChangesAsync();
        }
    }
}

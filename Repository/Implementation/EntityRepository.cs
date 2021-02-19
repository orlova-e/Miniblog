using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class EntityRepository : IPlainRepository<Entity>
    {
        public MiniblogDb Db { get; set; }
        public EntityRepository(MiniblogDb db)
        {
            Db = db;
        }
        public async Task CreateAsync(Entity entity)
        {
            Db.Entities.Add(entity);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            Entity entity = await Db.Entities.FindAsync(id);
            Db.Entities.Remove(entity);
            await Db.SaveChangesAsync();
        }

        public IEnumerable<Entity> Find(Func<Entity, bool> predicate)
        {
            return Db.Entities.Where(predicate);
        }

        public async Task<IEnumerable<Entity>> FindAsync(Func<Entity, bool> predicate)
        {
            return await Task.Run(() => Find(predicate));
        }

        public async Task<IEnumerable<Entity>> GetAllAsync()
        {
            return await Db.Entities.ToListAsync();
        }

        public async Task<Entity> GetByIdAsync(Guid id)
        {
            return await Db.Entities.FindAsync(id);
        }

        public async Task UpdateAsync(Entity entity)
        {
            Db.Entities.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

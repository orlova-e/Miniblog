using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class TopicsRepository : IPlainRepository<Topic>
    {
        public MiniblogDb Db { get; }
        public TopicsRepository(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<Topic> GetByIdAsync(Guid id)
        {
            return await Db.Topics.FindAsync(id);
        }
        public async Task<IEnumerable<Topic>> GetAllAsync()
        {
            return await Db.Topics.ToListAsync();
        }
        public IEnumerable<Topic> Find(Func<Topic, bool> predicate)
        {
            return Db.Topics.Where(predicate).ToList();
        }
        public async Task CreateAsync(Topic entity)
        {
            Db.Topics.Add(entity);
            await Db.SaveChangesAsync();
        }
        public async Task UpdateAsync(Topic entity)
        {
            Db.Topics.Update(entity);
            await Db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var topic = await Db.Topics.FindAsync(id);
            Db.Topics.Remove(topic);
            await Db.SaveChangesAsync();
        }
    }
}

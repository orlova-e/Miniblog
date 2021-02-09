using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class TagsRepository : ITagsRepository
    {
        public MiniblogDb Db { get; }
        public TagsRepository(MiniblogDb db)
        {
            Db = db;
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

        public IEnumerable<Tag> FindRange(params string[] query)
        {
            if (query?.Length == 0)
                return null;
            List<Tag> tags = new List<Tag>();
            var foundTags = Db.Tags.Where(t => query.Contains(t.Name));
            tags.AddRange(foundTags);
            return tags;
        }
    }
}

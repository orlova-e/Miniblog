using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class CommentsOptionsRepo : IOptionRepository<CommentsOptions>
    {
        public MiniblogDb Db { get; }
        public CommentsOptionsRepo(MiniblogDb db)
        {
            Db = db;
        }
        public IEnumerable<CommentsOptions> Find(Func<CommentsOptions, bool> predicate)
        {
            return Db.CommentsOptions.Where(predicate).ToList();
        }

        public async Task<CommentsOptions> FirstOrDefaultAsync()
        {
            return await Db.CommentsOptions.FirstOrDefaultAsync();
        }

        public async Task<CommentsOptions> GetByIdAsync(Guid id)
        {
            return await Db.CommentsOptions.FindAsync(id);
        }

        public async Task UpdateAsync(CommentsOptions entity)
        {
            Db.CommentsOptions.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

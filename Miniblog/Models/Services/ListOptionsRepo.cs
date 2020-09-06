using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class ListOptionsRepo : IOptionRepository<ListDisplayOptions>
    {
        public MiniblogDb Db { get; }
        public ListOptionsRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<ListDisplayOptions> GetByIdAsync(Guid id)
        {
            return await Db.ListDisplayOptions.FindAsync(id);
        }
        public async Task UpdateAsync(ListDisplayOptions entity)
        {
            Db.ListDisplayOptions.Update(entity);
            await Db.SaveChangesAsync();
        }

        public IEnumerable<ListDisplayOptions> Find(Func<ListDisplayOptions, bool> predicate)
        {
            return Db.ListDisplayOptions.Where(predicate).ToList();
        }

        public async Task<ListDisplayOptions> FirstOrDefaultAsync()
        {
            return (await Db.ListDisplayOptions.ToArrayAsync()).FirstOrDefault();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class WebsiteOptionsRepo : IOptionRepository<WebsiteOptions>
    {
        public MiniblogDb Db { get; }
        public WebsiteOptionsRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<WebsiteOptions> GetByIdAsync(Guid id)
        {
            return await Db.WebsiteOptions.FindAsync(id);
        }
        public async Task UpdateAsync(WebsiteOptions entity)
        {
            Db.Update(entity);
            await Db.SaveChangesAsync();
        }

        public IEnumerable<WebsiteOptions> Find(Func<WebsiteOptions, bool> predicate)
        {
            return Db.WebsiteOptions.Where(predicate).ToList();
        }

        public async Task<WebsiteOptions> FirstOrDefaultAsync()
        {
            return (await Db.WebsiteOptions.ToArrayAsync()).FirstOrDefault();
        }
    }
}

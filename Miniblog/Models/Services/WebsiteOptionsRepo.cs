using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class WebsiteOptionsRepo : IOptionRepository<WebsiteDisplayOptions>
    {
        public MiniblogDb Db { get; }
        public WebsiteOptionsRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<WebsiteDisplayOptions> GetByIdAsync(Guid id)
        {
            return await Db.WebsiteDisplayOptions.FindAsync(id);
        }
        public async Task UpdateAsync(WebsiteDisplayOptions entity)
        {
            Db.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

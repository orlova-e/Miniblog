using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class ArticlesListOptionsRepo : IOptionRepository<ArticlesListDisplayOptions>
    {
        public MiniblogDb Db { get; }
        public ArticlesListOptionsRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<ArticlesListDisplayOptions> GetByIdAsync(int id)
        {
            return await Db.ArticlesListDisplayOptions.FindAsync(id);
        }
        public async Task UpdateAsync(ArticlesListDisplayOptions entity)
        {
            Db.ArticlesListDisplayOptions.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

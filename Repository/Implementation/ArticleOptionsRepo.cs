using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class ArticleOptionsRepo : IOptionRepository<ArticleOptions>
    {
        public MiniblogDb Db { get; }
        public ArticleOptionsRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<ArticleOptions> GetByIdAsync(Guid id)
        {
            return await Db.ArticleOptions.FindAsync(id);
        }
        public async Task UpdateAsync(ArticleOptions entity)
        {
            Db.Update(entity);
            await Db.SaveChangesAsync();
        }

        public IEnumerable<ArticleOptions> Find(Func<ArticleOptions, bool> predicate)
        {
            return Db.ArticleOptions.Where(predicate).ToList();
        }

        public async Task<ArticleOptions> FirstOrDefaultAsync()
        {
            return (await Db.ArticleOptions.ToArrayAsync()).FirstOrDefault();
        }
    }
}

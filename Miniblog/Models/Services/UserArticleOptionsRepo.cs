using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class UserArticleOptionsRepo : IOptionRepository<UserArticleDisplayOptions>
    {
        public MiniblogDb Db { get; }
        public UserArticleOptionsRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<UserArticleDisplayOptions> GetByIdAsync(Guid id)
        {
            return await Db.UserArticleDisplayOptions.FindAsync(id);
        }
        public async Task UpdateAsync(UserArticleDisplayOptions entity)
        {
            Db.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

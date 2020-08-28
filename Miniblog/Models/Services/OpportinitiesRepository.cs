using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;

namespace Miniblog.Models.Services
{
    public class OpportinitiesRepository : IOptionRepository<Opportunities>
    {
        public MiniblogDb Db { get; }
        public OpportinitiesRepository(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<Opportunities> GetByIdAsync(int id)
        {
            var opportunity = await Db.Opportunities.FindAsync(id);
            await Db.Roles.Where(r => r.Opportunities == opportunity).LoadAsync();
            return opportunity;
        }
        public async Task UpdateAsync(Opportunities entity)
        {
            Db.Opportunities.Update(entity);
            await Db.SaveChangesAsync();
        }
    }
}

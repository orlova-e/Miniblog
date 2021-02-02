using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class CheckListsRepository : IOptionRepository<CheckList>
    {
        public MiniblogDb Db { get; }
        public CheckListsRepository(MiniblogDb db)
        {
            Db = db;
        }

        public async Task<CheckList> GetByIdAsync(Guid id)
        {
            return await Db.CheckLists.FindAsync(id);
        }

        public async Task<List<CheckList>> GetAllAsync()
        {
            return await Db.CheckLists.ToListAsync();
        }

        public IEnumerable<CheckList> Find(Func<CheckList, bool> predicate)
        {
            return Db.CheckLists.Where(predicate).ToList();
        }

        public async Task UpdateAsync(CheckList entity)
        {
            Db.CheckLists.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task<CheckList> FirstOrDefaultAsync()
        {
            return await Db.CheckLists.FirstOrDefaultAsync();
        }
    }
}

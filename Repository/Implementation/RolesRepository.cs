using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class RolesRepository : IOptionRepository<Role>
    {
        public MiniblogDb Db { get; }
        public RolesRepository(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<Role> GetByIdAsync(Guid id)
        {
            var role = await Db.Roles.FindAsync(id);
            if (role == null)
            {
                role = await Db.ExtendedRoles.FindAsync(id);
            }
            return role;
        }
        public IEnumerable<Role> Find(Func<Role, bool> predicate)
        {
            IEnumerable<Role> roles = Db.Roles.Where(predicate).ToList();
            return roles;
        }

        public async Task UpdateAsync(Role entity)
        {
            Db.Roles.Update(entity);
            await Db.SaveChangesAsync();
        }

        public async Task<Role> FirstOrDefaultAsync()
        {
            return await Db.Roles.FirstOrDefaultAsync();
        }
    }
}

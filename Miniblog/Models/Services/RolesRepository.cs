using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
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
        public async Task UpdateAsync(Role entity)
        {
            if(entity.GetType() == typeof(ExtendedRole))
            {
                Db.ExtendedRoles.Update(entity as ExtendedRole);
            }
            else
            {
                Db.Roles.Update(entity);
            }
            await Db.SaveChangesAsync();
        }
    }
}

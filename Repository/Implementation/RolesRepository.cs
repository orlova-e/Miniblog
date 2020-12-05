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
            IEnumerable<Role> roles;
            //if(predicate is Func<ExtendedRole, bool>)
            //{
            //    roles = Db.ExtendedRoles.Where(predicate).ToList();
            //}
            //else
            //{
                roles = Db.Roles.Where(predicate).ToList();
            //}
            return roles;
        }
        //Role FindFirst(Func<Role, bool> predicate)
        //{
        //    Role role;
        //    if(predicate is Func<ExtendedRole, bool>)
        //    {
        //        role = Db.ExtendedRoles.Where(predicate).First();
        //    }
        //    else
        //    {
        //        role = Db.Roles.Where(predicate).First();
        //    }
        //    return role;
        //}
        public async Task UpdateAsync(Role entity)
        {
            //if(entity.GetType() == typeof(ExtendedRole))
            //{
            //    Db.ExtendedRoles.Update(entity as ExtendedRole);
            //}
            //else
            //{
                Db.Roles.Update(entity);
            //}
            await Db.SaveChangesAsync();
        }

        public async Task<Role> FirstOrDefaultAsync()
        {
            return (await Db.Roles.ToArrayAsync()).FirstOrDefault();
        }
    }
}

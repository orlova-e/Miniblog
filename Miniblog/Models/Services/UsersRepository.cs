using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class UsersRepository : IPlainRepository<User>
    {
        public MiniblogDb Db { get; }
        public UsersRepository(MiniblogDb db)
        {
            Db = db;
        }
        /// <returns>User with role.</returns>
        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await Db.Users.FindAsync(id);
            await Db.Entry(user).Reference(u => u.Role).LoadAsync();
            //var role = (from r in await Db.Roles.ToArrayAsync()
            //            where r.Id == user.RoleId
            //            select r).First();
            //await Db.Entry(role).Reference(r => r.Opportunities).LoadAsync();
            return user;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Db.Users.ToListAsync();
        }
        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            return Db.Users.Where(predicate).ToList();
        }
        public async Task CreateAsync(User entity)
        {
            Db.Users.Add(entity);
            await Db.SaveChangesAsync();
        }
        public async Task UpdateAsync(User entity)
        {
            Db.Users.Update(entity);
            await Db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var user = await Db.Users.FindAsync(id);
            Db.Users.Remove(user);
            await Db.SaveChangesAsync();
        }
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class UsersRepository : IPlainRepository<User>
    {
        public MiniblogDb Db { get; }
        public UsersRepository(MiniblogDb db)
        {
            Db = db;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await Db.Users.FindAsync(id);
            await Db.Entry(user).Reference(u => u.Role).LoadAsync();
            await Db.Entry(user).Collection(u => u.Subscribers).LoadAsync();
            return user;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await Db.Users.ToListAsync();
        }
        public IEnumerable<User> Find(Func<User, bool> predicate)
        {
            return Db.Users
                .Include(u => u.Role)
                .Include(u => u.Subscribers)
                .Where(predicate)
                .ToList();
        }
        public async Task<IEnumerable<User>> FindAsync(Func<User, bool> predicate)
        {
            return await Task.Run(() => Db.Users.Include(u => u.Subscribers).Where(predicate).ToList());
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
            IEnumerable<Article> articles = Db.Articles.Where(a => a.UserId == id);
            foreach(var article in articles)
            {
                article.UserId = null;
            }
            Db.Articles.UpdateRange(articles);
            Db.Users.Remove(user);
            await Db.SaveChangesAsync();
        }
    }
}

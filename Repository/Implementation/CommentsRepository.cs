using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class CommentsRepository : IPlainRepository<Comment>
    {
        public MiniblogDb Db { get; }
        public CommentsRepository(MiniblogDb db)
        {
            Db = db;
        }
        /// <returns>Comment with its author and parent comment.</returns>
        public async Task<Comment> GetByIdAsync(Guid id)
        {
            var comment = await Db.Comments.FindAsync(id);
            await Db.Entry(comment).Reference(c => c.Author).LoadAsync();
            await Db.Entry(comment).Reference(c => c.Parent).LoadAsync();
            return comment;
        }
        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            return await Db.Comments.ToListAsync();
        }
        public IEnumerable<Comment> Find(Func<Comment, bool> predicate)
        {
            return Db.Comments.Include(c => c.Author)
                .Where(predicate)
                .ToList();
        }
        public async Task<IEnumerable<Comment>> FindAsync(Func<Comment, bool> predicate)
        {
            return await Task.Run(() => Db.Comments.Include(c => c.Author)
                .Where(predicate)
                .ToList());
        }
        public async Task CreateAsync(Comment entity)
        {
            Db.Comments.Add(entity);
            await Db.SaveChangesAsync();
        }
        public async Task UpdateAsync(Comment entity)
        {
            Db.Comments.Update(entity);
            await Db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var comment = await Db.Comments.FindAsync(id);
            Db.Comments.Remove(comment);
            await Db.SaveChangesAsync();
        }
    }
}

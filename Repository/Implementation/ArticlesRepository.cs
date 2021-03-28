using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class ArticlesRepository : IPlainRepository<Article>
    {
        public MiniblogDb Db { get; }
        public ArticlesRepository(MiniblogDb db)
        {
            Db = db;
        }
        /// <returns>Article with explicitly loaded images.</returns>
        public async Task<Article> GetByIdAsync(Guid id)
        {
            Article article = await Db.Articles
                .Where(a => a.Id == id)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.Likes)
                .Include(a => a.Images)
                .FirstOrDefaultAsync();
            return article;
        }
        /// <returns>List of articles with explicitly loaded images.</returns>
        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await Db.Articles.Include(a => a.Images).ToListAsync();
        }
        /// <returns>List of selected articles without explicitly loaded images.</returns>
        public IEnumerable<Article> Find(Func<Article, bool> predicate)
        {
            return Db.Articles
                .Include(a => a.User)
                .Include(a => a.DisplayOptions)
                .Include(a => a.Topic)
                .Include(a => a.Series)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .Include(a => a.Likes)
                .Include(a => a.Bookmarks)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.Likes)
                .Include(a => a.Images)
                .Where(predicate)
                .ToList();
        }
        public async Task<IEnumerable<Article>> FindAsync(Func<Article, bool> predicate)
        {
            return await Task.Run(() => Find(predicate));
        }
        public async Task CreateAsync(Article entity)
        {
            Db.Articles.Add(entity);
            await Db.SaveChangesAsync();
        }
        public async Task UpdateAsync(Article entity)
        {
            Db.Articles.Update(entity);
            await Db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var Article = await Db.Articles.FindAsync(id);
            Db.Articles.Remove(Article);
            await Db.SaveChangesAsync();
        }
    }
}

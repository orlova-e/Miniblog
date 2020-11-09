﻿using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
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
            /*var article = await Db.Articles.FindAsync(id)*/;
            Article article = await Db.Articles
                .Where(a => a.Id == id)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.Author)
                .Include(a => a.Images)
                .FirstOrDefaultAsync();
            //await Db.Entry(article).Reference(a => a.UserArticleDisplayOptions).LoadAsync();
            //await Db.Entry(article).Reference(a => a.Topic).LoadAsync();
            //await Db.Entry(article).Reference(a => a.Series).LoadAsync();
            //await Db.Entry(article).Collection(a => a.Comments).LoadAsync();

            //await Db.Entry(article).Collection(a => a.Images).LoadAsync();
            //await Db.Entry(article).Collection(a => a.Comments).LoadAsync();
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
                .Include(a=>a.User)
                .Where(predicate)
                .ToList();
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

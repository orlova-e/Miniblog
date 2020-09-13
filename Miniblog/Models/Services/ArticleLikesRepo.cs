using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class ArticleLikesRepo : IRelatedRepository<UserFavourite, Article>
    {
        public MiniblogDb Db { get; private set; }
        public ArticleLikesRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<IEnumerable<Article>> GetAllForAsync(Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Liked)
                .ThenInclude(l => l.Article)
                .ToArrayAsync())
                .FirstOrDefault();
            IEnumerable<Article> articles = new List<Article>();
            foreach(var like in user.Liked)
            {
                articles.Append(like.Article);
            }
            return articles;
        }

        public async Task<IEnumerable<User>> GetAllUserForAsync(Guid entryId)
        {
            var entry = (await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Likes)
                .ThenInclude(l => l.User)
                .ToArrayAsync())
                .FirstOrDefault();
            IEnumerable<User> users = new List<User>();
            foreach(var like in entry.Likes)
            {
                users.Append(like.User);
            }
            return users;
        }

        public async Task AddFor(Guid entryId, Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Liked)
                .ToArrayAsync())
                .FirstOrDefault();
            var article = Db.Articles.Find(entryId);
            user.Liked.Add(new UserFavourite() { User = user, Article = article });
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }

        public async Task RemoveFor(Guid entryId, Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Liked)
                .ThenInclude(l => l.Article.Id == entryId)
                .ToArrayAsync())
                .FirstOrDefault();
            var like = (from l in user.Liked
                        where l.ArticleId == entryId
                        select l).FirstOrDefault();
            user.Liked.Remove(like);
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }

        public async Task<List<UserFavourite>> GetAsync(Guid entryId)
        {
            var entry = (await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Likes)
                .ToArrayAsync())
                .FirstOrDefault();
            return entry.Likes;
        }

        public int Count(Guid entryId)
        {
            var article = Db.Articles.Where(a => a.Id == entryId).Include(a => a.Likes).FirstOrDefault();
            var number = article.Likes.Count;
            return number;
        }
    }
}

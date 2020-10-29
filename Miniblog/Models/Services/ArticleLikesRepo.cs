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
        public async Task<IEnumerable<Article>> GetAllEntriesForAsync(Guid userId)
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

        public async Task<IEnumerable<User>> GetAllUsersForAsync(Guid entryId)
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

        public async Task AddForAsync(Guid entryId, Guid userId)
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

        public async Task RemoveForAsync(Guid entryId, Guid userId)
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

        public async Task<bool> ContainsAsync(Guid entryId, Guid userId)
        {
            var relation = (from article in await Db.Articles.ToArrayAsync()
                            where article.Id.Equals(entryId)
                            from like in article.Likes
                            where like.UserId.Equals(userId)
                            select like).FirstOrDefault();
            if (relation != null)
                return true;
            return false;
        }
    }
}

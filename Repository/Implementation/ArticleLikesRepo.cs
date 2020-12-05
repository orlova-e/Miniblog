using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
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
            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Liked)
                .ThenInclude(l => l.Article)
                .FirstOrDefaultAsync();

            IEnumerable<Article> articles = user.Liked
                .Select(like => like.Article);

            return articles;
        }

        public async Task<IEnumerable<User>> GetAllUsersForAsync(Guid entryId)
        {
            Article article = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(c => c.Likes)
                .ThenInclude(like => like.User)
                .FirstOrDefaultAsync();

            IEnumerable<User> users = article.Likes
                .Select(like => like.User);

            return users;
        }

        public async Task AddForAsync(Guid entryId, Guid userId)
        {
            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Liked)
                .FirstOrDefaultAsync();

            Article article = Db.Articles.Find(entryId);
            user.Liked.Add(new UserFavourite() { User = user, Article = article });
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }

        public async Task RemoveForAsync(Guid entryId, Guid userId)
        {
            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Liked)
                .FirstOrDefaultAsync();

            UserFavourite userFavourite = user.Liked
                .Where(like => like.ArticleId == entryId)
                .FirstOrDefault();

            user.Liked.Remove(userFavourite);
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }

        public async Task<List<UserFavourite>> GetAsync(Guid entryId)
        {
            Article article = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Likes)
                .FirstOrDefaultAsync();
            return article.Likes;
        }

        public async Task<int> CountAsync(Guid entryId)
        {
            Article article = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Likes)
                .FirstOrDefaultAsync();

            int number = article.Likes.Count;
            return number;
        }

        public async Task<bool> ContainsAsync(Guid entryId, Guid userId)
        {
            List<UserFavourite> userFavourites = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Likes)
                .Select(a => a.Likes)
                .FirstOrDefaultAsync();

            UserFavourite relation = userFavourites
                .Where(uf => uf.UserId == userId)
                .FirstOrDefault();

            if (relation != null)
                return true;
            return false;
        }
    }
}

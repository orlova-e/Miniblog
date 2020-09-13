using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class ArticleBookmarksRepo : IRelatedRepository<UserBookmark, Article>
    {
        public MiniblogDb Db { get; private set; }
        public ArticleBookmarksRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<IEnumerable<Article>> GetAllForAsync(Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Bookmarked)
                .ThenInclude(l => l.Article)
                .ToArrayAsync())
                .FirstOrDefault();
            IEnumerable<Article> articles = new List<Article>();
            foreach (var bookmark in user.Bookmarked)
            {
                articles.Append(bookmark.Article);
            }
            return articles;
        }
        public async Task<IEnumerable<User>> GetAllUserForAsync(Guid entryId)
        {
            var entry = (await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Bookmarks)
                .ThenInclude(l => l.User)
                .ToArrayAsync())
                .FirstOrDefault();
            IEnumerable<User> users = new List<User>();
            foreach (var bookmark in entry.Bookmarks)
            {
                users.Append(bookmark.User);
            }
            return users;
        }
        public async Task AddFor(Guid entryId, Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Bookmarked)
                .ToArrayAsync())
                .FirstOrDefault();
            var article = Db.Articles.Find(entryId);
            user.Bookmarked.Add(new UserBookmark() { User = user, Article = article });
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
        public async Task RemoveFor(Guid entryId, Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Bookmarked)
                .ThenInclude(l => l.Article.Id == entryId)
                .ToArrayAsync())
                .FirstOrDefault();
            var like = (from l in user.Bookmarked
                        where l.ArticleId == entryId
                        select l).FirstOrDefault();
            user.Bookmarked.Remove(like);
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
        public async Task<List<UserBookmark>> GetAsync(Guid entryId)
        {
            var entry = (await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Bookmarks)
                .ToArrayAsync())
                .FirstOrDefault();
            return entry.Bookmarks;
        }
        public int Count(Guid entryId)
        {
            var article = Db.Articles.Where(a => a.Id == entryId).Include(a => a.Bookmarks).FirstOrDefault();
            var number = article.Bookmarks.Count;
            return number;
        }
    }
}

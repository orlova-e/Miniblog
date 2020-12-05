using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class ArticleBookmarksRepo : IRelatedRepository<UserBookmark, Article>
    {
        public MiniblogDb Db { get; private set; }
        public ArticleBookmarksRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<IEnumerable<Article>> GetAllEntriesForAsync(Guid userId)
        {
            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Bookmarked)
                .ThenInclude(l => l.Article)
                .FirstOrDefaultAsync();

            IEnumerable<Article> articles = user.Bookmarked
                .Select(b => b.Article);

            return articles;
        }
        public async Task<IEnumerable<User>> GetAllUsersForAsync(Guid entryId)
        {
            Article article = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Bookmarks)
                .ThenInclude(l => l.User)
                .FirstOrDefaultAsync();

            IEnumerable<User> users = article.Bookmarks
                .Select(b => b.User);

            return users;
        }
        public async Task AddForAsync(Guid entryId, Guid userId)
        {
            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Bookmarked)
                .FirstOrDefaultAsync();

            Article article = Db.Articles.Find(entryId);
            user.Bookmarked.Add(new UserBookmark() { User = user, Article = article });
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
        public async Task RemoveForAsync(Guid entryId, Guid userId)
        {
            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Bookmarked)
                .FirstOrDefaultAsync();

            UserBookmark bookmark = user.Bookmarked
                .Where(b => b.ArticleId == entryId)
                .FirstOrDefault();

            user.Bookmarked.Remove(bookmark);
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
        public async Task<List<UserBookmark>> GetAsync(Guid entryId)
        {
            Article article = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Bookmarks)
                .FirstOrDefaultAsync();

            return article.Bookmarks;
        }
        public async Task<int> CountAsync(Guid entryId)
        {
            Article article = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Bookmarks)
                .FirstOrDefaultAsync();

            int number = article.Bookmarks.Count;
            return number;
        }

        public async Task<bool> ContainsAsync(Guid entryId, Guid userId)
        {
            List<UserBookmark> userBookmarks = await Db.Articles
                .Where(a => a.Id == entryId)
                .Include(a => a.Bookmarks)
                .Select(a => a.Bookmarks)
                .FirstOrDefaultAsync();

            UserBookmark userBookmark = userBookmarks
                .Where(ub => ub.UserId == userId)
                .FirstOrDefault();

            if (userBookmark != null)
                return true;
            return false;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services
{
    public class CommentLikeRepo : IRelatedRepository<CommentLikes, Comment>
    {
        public MiniblogDb Db { get; private set; }
        public CommentLikeRepo(MiniblogDb db)
        {
            Db = db;
        }
        public async Task<IEnumerable<Comment>> GetAllForAsync(Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.LikedComments)
                .ThenInclude(l => l.Comment)
                .ToArrayAsync())
                .FirstOrDefault();
            IEnumerable<Comment> comments = new List<Comment>();
            foreach (var liked in user.LikedComments)
            {
                comments.Append(liked.Comment);
            }
            return comments;
        }
        public async Task<IEnumerable<User>> GetAllUserForAsync(Guid entryId)
        {
            var entry = (await Db.Comments
                .Where(c => c.Id == entryId)
                .Include(c => c.Likes)
                .ThenInclude(l => l.User)
                .ToArrayAsync())
                .FirstOrDefault();
            IEnumerable<User> users = new List<User>();
            foreach (var like in entry.Likes)
            {
                users.Append(like.User);
            }
            return users;
        }
        public async Task AddFor(Guid entryId, Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.LikedComments)
                .ToArrayAsync())
                .FirstOrDefault();
            var comment = Db.Comments.Find(entryId);
            user.LikedComments.Add(new CommentLikes() { User = user, Comment = comment });
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
        public async Task RemoveFor(Guid entryId, Guid userId)
        {
            var user = (await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.LikedComments)
                .ThenInclude(l => l.Comment.Id == entryId)
                .ToArrayAsync())
                .FirstOrDefault();
            var like = (from l in user.LikedComments
                        where l.CommentId == entryId
                        select l).FirstOrDefault();
            user.LikedComments.Remove(like);
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
        public async Task<List<CommentLikes>> GetAsync(Guid entryId)
        {
            var entry = (await Db.Comments
                .Where(a => a.Id == entryId)
                .Include(a => a.Likes)
                .ToArrayAsync())
                .FirstOrDefault();
            return entry.Likes;
        }
        public int Count(Guid entryId)
        {
            var article = Db.Articles.Where(a => a.Id == entryId).Include(a => a.Comments).FirstOrDefault();
            var number = article.Comments.Count;
            return number;
        }
    }
}

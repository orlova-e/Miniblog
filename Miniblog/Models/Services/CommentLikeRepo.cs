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
        public async Task<IEnumerable<Comment>> GetAllEntriesForAsync(Guid userId)
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
        public async Task<IEnumerable<User>> GetAllUsersForAsync(Guid entryId)
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
        public async Task AddForAsync(Guid entryId, Guid userId)
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
        public async Task RemoveForAsync(Guid entryId, Guid userId)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryId">Comment's id</param>
        /// <returns></returns>
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
            var comment = Db.Comments.Where(c => c.Id == entryId).Include(a => a.Likes).FirstOrDefault();
            var number = comment.Likes.Count;
            return number;
        }

        public async Task<bool> ContainsAsync(Guid entryId, Guid userId)
        {
            //var lik = Db.Comments
            //    .Where(c => c.Id.Equals(entryId))
            //    .Include(l => l.Likes
            //        .Where(l => l.UserId.Equals(userId))
            //        .FirstOrDefault());

            var relation = (from comment in await Db.Comments.ToArrayAsync()
                         where comment.Id.Equals(entryId)
                         from like in comment.Likes
                         where like.UserId.Equals(userId)
                         select like).FirstOrDefault();
            if (relation != null)
                return true;
            return false;
        }
    }
}

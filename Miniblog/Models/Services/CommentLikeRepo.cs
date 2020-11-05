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
            var user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.LikedComments)
                .ThenInclude(l => l.Comment)
                .FirstOrDefaultAsync();

            IEnumerable<Comment> comments = user.LikedComments
                .Select(like => like.Comment);

            //IEnumerable<Comment> comments = new List<Comment>();
            //foreach (var liked in user.LikedComments)
            //{
            //    comments.Append(liked.Comment);
            //}
            return comments;
        }
        public async Task<IEnumerable<User>> GetAllUsersForAsync(Guid entryId)
        {
            //var entry = (await Db.Comments
            //    .Where(c => c.Id == entryId)
            //    .Include(c => c.Likes)
            //    .ThenInclude(l => l.User)
            //    .ToArrayAsync())
            //    .FirstOrDefault();

            //IEnumerable<User> users = new List<User>();
            //foreach (var like in entry.Likes)
            //{
            //    users.Append(like.User);
            //}

            Comment comment = await Db.Comments
                .Where(c => c.Id == entryId)
                .Include(c => c.Likes)
                .ThenInclude(like => like.User)
                .FirstOrDefaultAsync();

            IEnumerable<User> users = comment.Likes
                .Select(like => like.User);

            return users;
        }
        public async Task AddForAsync(Guid entryId, Guid userId)
        {
            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.LikedComments)
                .FirstOrDefaultAsync();

            Comment comment = Db.Comments.Find(entryId);
            user.LikedComments.Add(new CommentLikes() { User = user, Comment = comment });
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }
        public async Task RemoveForAsync(Guid entryId, Guid userId)
        {
            //var user = (await Db.Users
            //    .Where(u => u.Id == userId)
            //    .Include(u => u.LikedComments)
            //    .ThenInclude(l => l.Comment.Id == entryId)
            //    .ToArrayAsync())
            //    .FirstOrDefault();
            //CommentLikes like = (from l in user.LikedComments
            //            where l.CommentId == entryId
            //            select l).FirstOrDefault();

            User user = await Db.Users
                .Where(u => u.Id == userId)
                .Include(u => u.LikedComments)
                .FirstOrDefaultAsync();

            CommentLikes commentLike = user.LikedComments
                .Where(like => like.CommentId == entryId)
                .FirstOrDefault();

            user.LikedComments.Remove(commentLike);
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
            Comment comment = await Db.Comments
                .Where(a => a.Id == entryId)
                .Include(a => a.Likes)
                .FirstOrDefaultAsync();
            return comment.Likes;
        }
        public async Task<int> CountAsync(Guid entryId)
        {
            Comment comment = await Db.Comments
                .Where(c => c.Id == entryId)
                .Include(a => a.Likes)
                .FirstOrDefaultAsync();

            int number = comment.Likes.Count;
            return number;
        }

        public async Task<bool> ContainsAsync(Guid entryId, Guid userId)
        {
            //CommentLikes relation = Db.Comments
            //    .Where(c => c.Id.Equals(entryId))
            //    .Include(l => l.Likes
            //        .Where(l => l.UserId.Equals(userId))
            //        .FirstOrDefault());


            //var relation = (from comment in await Db.Comments.ToArrayAsync()
            //             where comment.Id.Equals(entryId)
            //             from like in comment.Likes
            //             where like.UserId.Equals(userId)
            //             select like).FirstOrDefault();

            List<CommentLikes> commentLikes = await Db.Comments
                .Where(c => c.Id == entryId)
                .Include(c => c.Likes)
                .Select(c => c.Likes)
                .FirstOrDefaultAsync();

            CommentLikes commentLike = commentLikes
                .Where(cl => cl.UserId == userId)
                .FirstOrDefault();

            if (commentLike != null)
                return true;
            return false;
        }
    }
}

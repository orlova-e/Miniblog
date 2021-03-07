using Domain.Entities;
using Repo.Interfaces;
using Services.Interfaces;
using Services.VisibleValues;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class CommentsService : ICommentsService
    {
        private IRepository Repository { get; }
        private ITextService TextService { get; }
        private IEntityObserver EntityObserver { get; }

        public CommentsService(IRepository repository,
            IEntityObserver entityObserver)
        {
            Repository = repository;
            EntityObserver = entityObserver;
            TextService = new TextService();
        }

        public CommentsService(ITextService textService,
            IRepository repository,
            IEntityObserver entityObserver)
        {
            TextService = textService;
            Repository = repository;
            EntityObserver = entityObserver;
        }

        public async Task<Comment> AddCommentAsync(string username, string title, string text, Guid? parentId = null)
        {
            User user = Repository.Users.Find(u => u.Username == username).FirstOrDefault();

            if (!(user?.Role?.WriteComments ?? false))
                throw new InvalidOperationException();

            Article article = Repository.Articles.Find(a => a.Link == title).FirstOrDefault();
            if (article is null)
                throw new InvalidOperationException();

            text = TextService.GetPrepared(text);

            Comment parentComment = null;
            if (parentId is not null)
            {
                parentComment = article.Comments.Find(c => c.Id == (Guid)parentId);
                if (parentComment is null)
                    throw new InvalidOperationException();
            }

            Comment comment = new Comment
            {
                //Id = Guid.NewGuid(),
                Author = user,
                Article = article,
                Text = text,
                DateTime = DateTimeOffset.UtcNow,
                Parent = parentComment
            };

            if (parentComment is not null)
            {
                parentComment.Children.Add(comment);
            }
            await Repository.Comments.CreateAsync(comment);
            if (parentComment is not null)
            {
                await Repository.Comments.UpdateAsync(parentComment);
            }

            await EntityObserver.OnNewEntryAsync((VisibleCommentValues)comment);

            return comment;
        }

        public async Task<Comment> UpdateCommentAsync(string username, string text, Guid commentId)
        {
            User user = Repository.Users.Find(u => u.Username == username).FirstOrDefault();

            if (!(user?.Role?.WriteComments ?? false))
                throw new InvalidOperationException();

            Comment comment = await Repository.Comments.GetByIdAsync(commentId);

            if (comment is null || user.Id != comment.AuthorId || comment.IsDeleted)
                throw new InvalidOperationException();

            text = TextService.GetPrepared(text);
            comment.Text = text;
            comment.UpdatedDateTime = DateTimeOffset.UtcNow;
            await Repository.Comments.UpdateAsync(comment);
            await EntityObserver.OnUpdateAsync((VisibleCommentValues)comment);

            return comment;
        }

        public async Task DeleteCommentAsync(string username, Guid commentId)
        {
            User user = Repository.Users.Find(u => u.Username == username).FirstOrDefault();
            Comment comment = await Repository.Comments.GetByIdAsync(commentId);
            if (comment?.AuthorId != user?.Id || !(user.Role as ExtendedRole).ModerateComments || comment.IsDeleted)
            {
                throw new InvalidOperationException();
            }

            await DeleteAsync(comment);
        }

        public async Task DeleteCommentAsync(ExtendedRole extendedRole, Guid commentId)
        {
            Comment comment = await Repository.Comments.GetByIdAsync(commentId);

            if (comment is null || !extendedRole.ModerateComments || comment.IsDeleted)
            {
                throw new InvalidOperationException();
            }

            await DeleteAsync(comment);
        }

        private async Task DeleteAsync(Comment comment)
        {
            comment.IsDeleted = true;
            comment.Accepted = true;
            comment.VerifiedMatches = string.Empty;
            comment.Text = string.Empty;
            comment.UpdatedDateTime = DateTimeOffset.UtcNow;
            await Repository.Comments.UpdateAsync(comment);

            await EntityObserver.OnDeleteAsync((VisibleCommentValues)comment);
        }

        public async Task<bool> LikeComment(string username, Guid commentId)
        {
            User user = Repository.Users.Find(u => u.Username == username).FirstOrDefault();
            Comment comment = await Repository.Comments.GetByIdAsync(commentId);

            if (comment is null || user is null)
                throw new InvalidOperationException();

            bool canBeLiked = !await Repository.CommentLikes.ContainsAsync(comment.Id, user.Id) && !comment.IsDeleted;
            if (canBeLiked)
            {
                await Repository.CommentLikes.AddForAsync(comment.Id, user.Id);
            }
            else
            {
                await Repository.CommentLikes.RemoveForAsync(comment.Id, user.Id);
            }

            return canBeLiked;
        }
    }
}

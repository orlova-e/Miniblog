using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Repo.Interfaces;
using Services.Interfaces;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Hubs
{
    [Authorize]
    public class ArticleHub : Hub
    {
        readonly string DateTimePattern = new DateTimeFormatInfo().RoundtripDtPattern();
        public IRepository Repository { get; }
        public IArticleService ArticleService { get; }
        public IUserService UserService { get; }
        public ICommentsService CommentsService { get; }

        public ArticleHub(IRepository repository,
            IArticleService articlesService,
            IUserService userService,
            ICommentsService commentsService)
        {
            Repository = repository;
            ArticleService = articlesService;
            UserService = userService;
            CommentsService = commentsService;
        }

        public async Task AddComment(string title, string text, Guid? parentId = null)
        {
            Comment comment = await CommentsService.AddCommentAsync(Context.UserIdentifier, title, text, parentId);

            string avatar = null;
            if (comment.Author.Avatar is not null)
                avatar = Convert.ToBase64String(comment.Author.Avatar);

            CommentViewModel newComment = new CommentViewModel
            {
                ArticleId = comment.ArticleId.ToString(),
                CommentId = comment.Id.ToString(),
                ParentId = comment.ParentId.ToString(),
                Author = comment.Author.Username,
                Avatar = avatar,
                DateTime = comment.DateTime.ToString(DateTimePattern),
                Text = text
            };

            int commentsNumber = Repository.Comments
                .Find(c => c.ArticleId == comment.ArticleId)
                .Count();

            await Clients.All.SendAsync("AddedComment", newComment, commentsNumber);
            //await Clients.All.SendAsync("CommentsCounted", article.Link, commentsNumber);   // to method from articles' list
        }

        public async Task UpdateComment(string title, string text, Guid commentId)
        {
            Comment comment = await CommentsService.UpdateCommentAsync(Context.UserIdentifier, text, commentId);

            string avatar = null;
            if (comment.Author.Avatar is not null)
                avatar = Convert.ToBase64String(comment.Author.Avatar);

            CommentViewModel updatedComment = new CommentViewModel
            {
                ArticleId = comment.ArticleId.ToString(),
                CommentId = comment.Id.ToString(),
                ParentId = comment.ParentId.ToString(),
                Author = comment.Author.Username,
                Avatar = avatar,
                DateTime = comment.DateTime.ToString(DateTimePattern),
                UpdatedDateTime = comment.UpdatedDateTime?.ToString(DateTimePattern),
                Text = comment.Text
            };

            await Clients.All.SendAsync("UpdatedComment", updatedComment);
        }

        public async Task DeleteComment(string title, Guid commentId)
        {
            await CommentsService.DeleteCommentAsync(Context.UserIdentifier, commentId);
            Comment comment = await Repository.Comments.GetByIdAsync(commentId);

            string avatar = null;
            if (comment.Author.Avatar != null)
                avatar = Convert.ToBase64String(comment.Author.Avatar);

            CommentViewModel deletedComment = new CommentViewModel
            {
                ArticleId = comment.ParentId.ToString(),
                CommentId = comment.Id.ToString(),
                ParentId = comment.ParentId.ToString(),
                Author = comment.Author.Username,
                Avatar = avatar,
                DateTime = comment.DateTime.ToString(DateTimePattern),
                UpdatedDateTime = comment.UpdatedDateTime?.ToString(DateTimePattern),
                Text = comment.Text
            };

            await Clients.All.SendAsync("DeletedComment", deletedComment);
        }

        public async Task LikeArticle(string title)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await UserService.GetFromDbAsync(userId);
            if (user == null)
                return;

            Article article = await ArticleService.GetArticleByLinkAsync(title);
            if (article != null)
            {
                if (!await Repository.ArticleLikes.ContainsAsync(article.Id, userId))
                {
                    await Repository.ArticleLikes.AddForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeIsChanged", true);
                }
                else
                {
                    await Repository.ArticleLikes.RemoveForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeIsChanged", false);
                }
                int number = await Repository.ArticleLikes.CountAsync(article.Id);
                await Clients.All.SendAsync("ArticleLikesCounted", article.Link, number);
            }
        }

        public async Task BookmarkArticle(string title)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await UserService.GetFromDbAsync(userId);
            if (user == null)
                return;

            Article article = await ArticleService.GetArticleByLinkAsync(title);
            if (article != null)
            {
                if (!await Repository.ArticleBookmarks.ContainsAsync(article.Id, userId))
                {
                    await Repository.ArticleBookmarks.AddForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkIsChanged", true);
                }
                else
                {
                    await Repository.ArticleBookmarks.RemoveForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkIsChanged", false);
                }
                int number = await Repository.ArticleBookmarks.CountAsync(article.Id);
                await Clients.All.SendAsync("ArticleBookmarksCounted", article.Link, number);
            }
        }

        public async Task LikeComment(Guid commentId)
        {
            bool isLiked = await CommentsService.LikeComment(Context.UserIdentifier, commentId);
            await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeIsChanged", commentId, isLiked);

            int number = await Repository.CommentLikes.CountAsync(commentId);
            await Clients.All.SendAsync("CommentLikesCounted", commentId, number);
        }
    }
}

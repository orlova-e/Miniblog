using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Repo.Interfaces;
using Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Hubs
{
    [Authorize]
    public class ArticleHub : Hub
    {
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

        private string GetTitle() => Context.GetHttpContext().Request.Query["title"];

        public override async Task OnConnectedAsync() => await Groups.AddToGroupAsync(Context.ConnectionId, GetTitle());

        public async Task AddComment(string text, Guid? parentId = null)
        {
            Comment comment = await CommentsService.AddCommentAsync(Context.UserIdentifier, GetTitle(), text, parentId);

            string avatar = null;
            if (comment.Author?.Avatar is not null)
                avatar = Convert.ToBase64String(comment.Author.Avatar);

            CommentViewModel newComment = new CommentViewModel
            {
                ArticleId = comment.ArticleId.ToString(),
                CommentId = comment.Id.ToString(),
                ParentId = comment.ParentId.ToString(),
                Author = comment.Author?.Username,
                Avatar = avatar,
                DateTime = comment.DateTime.UtcDateTime.ToString("o"),
                Text = text
            };

            int commentsNumber = Repository.Comments
                .Find(c => c.ArticleId == comment.ArticleId)
                .Count();

            await Clients.Group(GetTitle()).SendAsync("AddedComment", newComment, commentsNumber);
        }

        public async Task UpdateComment(string text, Guid commentId)
        {
            Comment comment = await CommentsService.UpdateCommentAsync(Context.UserIdentifier, text, commentId);

            string avatar = null;
            if (comment.Author?.Avatar is not null)
                avatar = Convert.ToBase64String(comment.Author.Avatar);

            CommentViewModel updatedComment = new CommentViewModel
            {
                ArticleId = comment.ArticleId.ToString(),
                CommentId = comment.Id.ToString(),
                ParentId = comment.ParentId.ToString(),
                Author = comment.Author?.Username,
                Avatar = avatar,
                DateTime = comment.DateTime.UtcDateTime.ToString("o"),
                UpdatedDateTime = comment.UpdatedDateTime?.UtcDateTime.ToString("o"),
                Text = comment.Text
            };

            await Clients.Group(GetTitle()).SendAsync("UpdatedComment", updatedComment);
        }

        public async Task DeleteComment(Guid commentId)
        {
            await CommentsService.DeleteCommentAsync(Context.UserIdentifier, commentId);
            Comment comment = await Repository.Comments.GetByIdAsync(commentId);

            string avatar = null;
            if (comment.Author?.Avatar is not null)
                avatar = Convert.ToBase64String(comment.Author.Avatar);

            CommentViewModel deletedComment = new CommentViewModel
            {
                ArticleId = comment.ParentId.ToString(),
                CommentId = comment.Id.ToString(),
                ParentId = comment.ParentId.ToString(),
                Author = comment.Author?.Username,
                Avatar = avatar,
                DateTime = comment.DateTime.UtcDateTime.ToString("o"),
                UpdatedDateTime = comment.UpdatedDateTime?.UtcDateTime.ToString("o"),
                Text = comment.Text
            };

            await Clients.Group(GetTitle()).SendAsync("DeletedComment", deletedComment);
        }

        public async Task LikeArticle()
        {
            User user = UserService.FindByName(Context.UserIdentifier);
            Article article = await ArticleService.GetArticleByLinkAsync(GetTitle());
            if (user is null || article is null)
                return;

            if (!await Repository.ArticleLikes.ContainsAsync(article.Id, user.Id))
            {
                await Repository.ArticleLikes.AddForAsync(article.Id, user.Id);
                await Clients.Caller.SendAsync("ArticleLikeIsChanged", true);
            }
            else
            {
                await Repository.ArticleLikes.RemoveForAsync(article.Id, user.Id);
                await Clients.Caller.SendAsync("ArticleLikeIsChanged", false);
            }
            int number = await Repository.ArticleLikes.CountAsync(article.Id);
            await Clients.Group(GetTitle()).SendAsync("ArticleLikesCounted", number);
        }

        public async Task BookmarkArticle()
        {
            User user = UserService.FindByName(Context.UserIdentifier);
            Article article = await ArticleService.GetArticleByLinkAsync(GetTitle());
            if (article is null || user is null)
                return;

            if (!await Repository.ArticleBookmarks.ContainsAsync(article.Id, user.Id))
            {
                await Repository.ArticleBookmarks.AddForAsync(article.Id, user.Id);
                await Clients.Caller.SendAsync("ArticleBookmarkIsChanged", true);
            }
            else
            {
                await Repository.ArticleBookmarks.RemoveForAsync(article.Id, user.Id);
                await Clients.Caller.SendAsync("ArticleBookmarkIsChanged", false);
            }
            int number = await Repository.ArticleBookmarks.CountAsync(article.Id);
            await Clients.Group(GetTitle()).SendAsync("ArticleBookmarksCounted", number);
        }

        public async Task LikeComment(Guid commentId)
        {
            bool isLiked = await CommentsService.LikeComment(Context.UserIdentifier, commentId);
            await Clients.Caller.SendAsync("CommentLikeIsChanged", commentId, isLiked);

            int number = await Repository.CommentLikes.CountAsync(commentId);
            await Clients.Group(GetTitle()).SendAsync("CommentLikesCounted", commentId, number);
        }
    }
}

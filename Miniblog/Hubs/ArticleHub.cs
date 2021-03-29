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

        private string GetArticleId()
        {
            string id = Context.GetHttpContext().Request.Query["articleId"];
            if (!Guid.TryParse(id, out Guid guid))
                throw new ArgumentException();
            return id;
        }

        public override async Task OnConnectedAsync() => await Groups.AddToGroupAsync(Context.ConnectionId, GetArticleId());

        public async Task AddComment(string text, Guid? parentId = null)
        {
            Guid articleId = Guid.Parse(GetArticleId());
            Comment comment = await CommentsService.AddCommentAsync(Context.UserIdentifier, articleId, text, parentId);

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
                Text = text,
                Requirements = true
            };

            int commentsNumber = Repository.Comments
                .Find(c => c.ArticleId == comment.ArticleId)
                .Count();

            await Clients.Caller.SendAsync("AddedComment", newComment, commentsNumber);
            newComment.Requirements = null;
            await Clients.GroupExcept(GetArticleId(), Context.ConnectionId).SendAsync("AddedComment", newComment, commentsNumber);
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
                Text = comment.Text,
                Requirements = true
            };
            await Clients.Caller.SendAsync("UpdatedComment", updatedComment);
            updatedComment.Requirements = null;
            await Clients.GroupExcept(GetArticleId(), Context.ConnectionId).SendAsync("UpdatedComment", updatedComment);
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
                Text = comment.Text,
                Requirements = false
            };

            await Clients.Group(GetArticleId()).SendAsync("DeletedComment", deletedComment);
        }

        public async Task LikeArticle()
        {
            User user = UserService.FindByName(Context.UserIdentifier);
            Guid id = Guid.Parse(GetArticleId());
            Article article = await Repository.Articles.GetByIdAsync(id);
            if (user is null || article is null)
                return;

            if (!await Repository.ArticleLikes.ContainsAsync(article.Id, user.Id))
            {
                await Repository.ArticleLikes.AddForAsync(article.Id, user.Id);
                await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeIsChanged", GetArticleId(), true);
            }
            else
            {
                await Repository.ArticleLikes.RemoveForAsync(article.Id, user.Id);
                await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeIsChanged", GetArticleId(), false);
            }
            int number = await Repository.ArticleLikes.CountAsync(article.Id);
            await Clients.Group(GetArticleId()).SendAsync("ArticleLikesCounted", number);
        }

        public async Task BookmarkArticle()
        {
            User user = UserService.FindByName(Context.UserIdentifier);
            Guid id = Guid.Parse(GetArticleId());
            Article article = await Repository.Articles.GetByIdAsync(id);
            if (article is null || user is null)
                return;

            if (!await Repository.ArticleBookmarks.ContainsAsync(article.Id, user.Id))
            {
                await Repository.ArticleBookmarks.AddForAsync(article.Id, user.Id);
                await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkIsChanged", GetArticleId(), true);
            }
            else
            {
                await Repository.ArticleBookmarks.RemoveForAsync(article.Id, user.Id);
                await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkIsChanged", GetArticleId(), false);
            }
            int number = await Repository.ArticleBookmarks.CountAsync(article.Id);
            await Clients.Group(GetArticleId()).SendAsync("ArticleBookmarksCounted", number);
        }

        public async Task LikeComment(Guid commentId)
        {
            bool isLiked = await CommentsService.LikeComment(Context.UserIdentifier, commentId);
            await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeIsChanged", GetArticleId(), commentId, isLiked);

            int number = await Repository.CommentLikes.CountAsync(commentId);
            await Clients.Group(GetArticleId()).SendAsync("CommentLikesCounted", commentId, number);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Domain.Entities;
using Repo.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Services.Interfaces;

namespace Miniblog.Hubs
{
    [Authorize]
    public class ArticleHub : Hub
    {
        string DateTimePattern { get; }
        public IRepository repository { get; private set; }
        public IArticlesService articlesService { get; private set; }
        public ITextService textService { get; private set; }
        public IUserService userService { get; private set; }

        public ArticleHub(IRepository repository,
            IArticlesService articlesService,
            ITextService textService,
            IUserService userService)
        {
            this.repository = repository;
            this.articlesService = articlesService;
            this.textService = textService;
            this.userService = userService;
            DateTimePattern = new DateTimeFormatInfo().RoundtripDtPattern();
        }

        public async Task AddComment(string title, string text, string parentId = null)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await userService.GetFromDbAsync(userId);
            if (!(user?.Role?.WriteComments ?? false))
                return;
            
            text = textService.GetPrepared(text);

            Article article = await articlesService.GetArticleByLinkAsync(title);

            if (article != null)
            {
                Comment parentComment = null;
                if (Guid.TryParse(parentId, out Guid parentGuid))
                {
                    parentComment = await repository.Comments.GetByIdAsync(parentGuid);
                    if (!article.Comments.Contains(parentComment))
                        return;
                }
                Comment comment = new Comment()
                {
                    Id = Guid.NewGuid(),
                    Author = user,
                    Article = article,
                    Text = text,
                    DateTime = DateTimeOffset.UtcNow,
                    Parent = parentComment
                };
                if (parentComment != null)
                    parentComment.Children.Add(comment);
                await repository.Comments.CreateAsync(comment);
                if (parentComment != null)
                    await repository.Comments.UpdateAsync(parentComment);
                string avatar = null;
                if (comment.Author.Avatar != null)
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
                int commentsNumber = repository.Comments
                    .Find(c => c.ArticleId == article.Id)
                    .Count();
                await Clients.All.SendAsync("AddedComment", newComment, commentsNumber);        // to method from article's page
                await Clients.All.SendAsync("CommentsCounted", article.Link, commentsNumber);   // to method from articles' list
            }
        }

        public async Task UpdateComment(string title, string text, string commentId)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await userService.GetFromDbAsync(userId);
            if (!(user?.Role?.WriteComments ?? false))
                return;

            Article article = await articlesService.GetArticleByLinkAsync(title);

            if(article != null)
            {
                if(Guid.TryParse(commentId, out Guid commentGuid))
                {
                    Comment comment = await repository.Comments.GetByIdAsync(commentGuid);
                    if (userId.Equals(comment.AuthorId) && !comment.IsDeleted && article.Comments.Contains(comment))
                    {
                        text = textService.GetPrepared(text);
                        comment.Text = text;
                        comment.UpdatedDateTime = DateTimeOffset.UtcNow;
                        await repository.Comments.UpdateAsync(comment);
                        string avatar = null;
                        if (comment.Author.Avatar != null)
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
                }
            }
        }

        public async Task DeleteComment(string title, string commentId)
        {
            Article article = await articlesService.GetArticleByLinkAsync(title);

            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await userService.GetFromDbAsync(userId);

            if (user == null || article == null || article.User == null)
                return;

            if ((!(user.Role?.WriteComments ?? default)
                && !article.User.Username.Equals(user.Username))
                || (!(user.Role as ExtendedRole)?.ModerateComments ?? default))
                return;

            if(Guid.TryParse(commentId, out Guid commentGuid))
            {
                Comment comment = await repository.Comments.GetByIdAsync(commentGuid);
                if(comment != null && !comment.IsDeleted && article.Comments.Contains(comment))
                {
                    comment.IsDeleted = true;
                    comment.Text = string.Empty;
                    comment.UpdatedDateTime = DateTimeOffset.UtcNow;
                    await repository.Comments.UpdateAsync(comment);
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
            }
        }
        public async Task LikeArticle(string title)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await userService.GetFromDbAsync(userId);
            if (user == null)
                return;

            Article article = await articlesService.GetArticleByLinkAsync(title);
            if(article != null)
            {
                if(!await repository.ArticleLikes.ContainsAsync(article.Id, userId))
                {
                    await repository.ArticleLikes.AddForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeIsChanged", true);
                }
                else
                {
                    await repository.ArticleLikes.RemoveForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeIsChanged", false);
                }
                int number = await repository.ArticleLikes.CountAsync(article.Id);
                await Clients.All.SendAsync("ArticleLikesCounted", article.Link, number);
            }
        }

        public async Task BookmarkArticle(string title)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await userService.GetFromDbAsync(userId);
            if (user == null)
                return;

            Article article = await articlesService.GetArticleByLinkAsync(title);
            if(article != null)
            {
                if(!await repository.ArticleBookmarks.ContainsAsync(article.Id, userId))
                {
                    await repository.ArticleBookmarks.AddForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkIsChanged", true);
                }
                else
                {
                    await repository.ArticleBookmarks.RemoveForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkIsChanged", false);
                }
                int number = await repository.ArticleBookmarks.CountAsync(article.Id);
                await Clients.All.SendAsync("ArticleBookmarksCounted", article.Link, number);
            }
        }

        public async Task LikeComment(string commentId)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await userService.GetFromDbAsync(userId);
            if (user == null)
                return;

            if(Guid.TryParse(commentId, out Guid commentGuid))
            {
                Comment comment = await repository.Comments.GetByIdAsync(commentGuid);
                if (comment is object)
                {
                    if (!await repository.CommentLikes.ContainsAsync(commentGuid, userId) && !comment.IsDeleted)
                    {
                        await repository.CommentLikes.AddForAsync(commentGuid, userId);
                        await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeIsChanged", commentId, true);
                    }
                    else
                    {
                        await repository.CommentLikes.RemoveForAsync(commentGuid, userId);
                        await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeIsChanged", commentId, false);
                    }
                    int number = await repository.CommentLikes.CountAsync(commentGuid);
                    await Clients.All.SendAsync("CommentLikesCounted", commentId, number);
                }
            }
        }
    }
}

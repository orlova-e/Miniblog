﻿using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Repo.Interfaces;
using Services.Interfaces;
using Services.VisibleValues;
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
        string DateTimePattern { get; }
        public IRepository Repository { get; }
        public IArticleService ArticleService { get; }
        public ITextService TextService { get; }
        public IUserService UserService { get; }
        public IEntityObserver EntityObserver { get; }

        public ArticleHub(IRepository repository,
            IArticleService articlesService,
            ITextService textService,
            IUserService userService,
            IEntityObserver entityObserver)
        {
            Repository = repository;
            ArticleService = articlesService;
            TextService = textService;
            UserService = userService;
            EntityObserver = entityObserver;
            DateTimePattern = new DateTimeFormatInfo().RoundtripDtPattern();
        }

        public async Task AddComment(string title, string text, string parentId = null)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await UserService.GetFromDbAsync(userId);
            if (!(user?.Role?.WriteComments ?? false))
                return;

            text = TextService.GetPrepared(text);

            Article article = await ArticleService.GetArticleByLinkAsync(title);

            if (article != null)
            {
                Comment parentComment = null;
                if (Guid.TryParse(parentId, out Guid parentGuid))
                {
                    parentComment = await Repository.Comments.GetByIdAsync(parentGuid);
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
                await Repository.Comments.CreateAsync(comment);
                if (parentComment != null)
                    await Repository.Comments.UpdateAsync(parentComment);
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
                int commentsNumber = Repository.Comments
                    .Find(c => c.ArticleId == article.Id)
                    .Count();

                await EntityObserver.OnNewEntryAsync((VisibleCommentValues)comment);
                await Clients.All.SendAsync("AddedComment", newComment, commentsNumber);        // to method from article's page
                await Clients.All.SendAsync("CommentsCounted", article.Link, commentsNumber);   // to method from articles' list
            }
        }

        public async Task UpdateComment(string title, string text, string commentId)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await UserService.GetFromDbAsync(userId);
            if (!(user?.Role?.WriteComments ?? false))
                return;

            Article article = await ArticleService.GetArticleByLinkAsync(title);

            if (article != null)
            {
                if (Guid.TryParse(commentId, out Guid commentGuid))
                {
                    Comment comment = await Repository.Comments.GetByIdAsync(commentGuid);
                    if (userId.Equals(comment.AuthorId) && !comment.IsDeleted && article.Comments.Contains(comment))
                    {
                        text = TextService.GetPrepared(text);
                        comment.Text = text;
                        comment.UpdatedDateTime = DateTimeOffset.UtcNow;
                        await Repository.Comments.UpdateAsync(comment);
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
                        await EntityObserver.OnUpdateAsync((VisibleCommentValues)comment);
                        await Clients.All.SendAsync("UpdatedComment", updatedComment);
                    }
                }
            }
        }

        public async Task DeleteComment(string title, string commentId)
        {
            Article article = await ArticleService.GetArticleByLinkAsync(title);

            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await UserService.GetFromDbAsync(userId);

            if (user == null || article == null || article.User == null)
                return;

            if ((!(user.Role?.WriteComments ?? default)
                && !article.User.Username.Equals(user.Username))
                || (!(user.Role as ExtendedRole)?.ModerateComments ?? default))
                return;

            if (Guid.TryParse(commentId, out Guid commentGuid))
            {
                Comment comment = await Repository.Comments.GetByIdAsync(commentGuid);
                if (comment != null && !comment.IsDeleted && article.Comments.Contains(comment))
                {
                    comment.IsDeleted = true;
                    comment.Text = string.Empty;
                    comment.UpdatedDateTime = DateTimeOffset.UtcNow;
                    await Repository.Comments.UpdateAsync(comment);
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
                    await EntityObserver.OnDeleteAsync((VisibleCommentValues)comment);
                    await Clients.All.SendAsync("DeletedComment", deletedComment);
                }
            }
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

        public async Task LikeComment(string commentId)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await UserService.GetFromDbAsync(userId);
            if (user == null)
                return;

            if (Guid.TryParse(commentId, out Guid commentGuid))
            {
                Comment comment = await Repository.Comments.GetByIdAsync(commentGuid);
                if (comment is object)
                {
                    if (!await Repository.CommentLikes.ContainsAsync(commentGuid, userId) && !comment.IsDeleted)
                    {
                        await Repository.CommentLikes.AddForAsync(commentGuid, userId);
                        await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeIsChanged", commentId, true);
                    }
                    else
                    {
                        await Repository.CommentLikes.RemoveForAsync(commentGuid, userId);
                        await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeIsChanged", commentId, false);
                    }
                    int number = await Repository.CommentLikes.CountAsync(commentGuid);
                    await Clients.All.SendAsync("CommentLikesCounted", commentId, number);
                }
            }
        }
    }
}

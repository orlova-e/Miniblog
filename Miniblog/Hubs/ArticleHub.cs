using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Hubs
{
    [Authorize]
    public class ArticleHub : Hub
    {
        //public User user { get; set; }
        string DatePattern { get; }
        string TimePattern { get; }
        public IRepository repository { get; private set; }
        public ITextService textService { get; private set; }

        public ArticleHub(IRepository repository, ITextService textService)
        {
            this.repository = repository;
            this.textService = textService;
            DatePattern = CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern;
            TimePattern = CultureInfo.InvariantCulture.DateTimeFormat.ShortTimePattern;
        }

        public async Task AddComment(string title, string text, string parentId = null)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await repository.Users.GetByIdAsync(userId);
            if (!(user?.Role?.WriteComments ?? false))
                return;
            
            text = textService.GetPrepared(text);

            var httpContext = Context.GetHttpContext();
            //string link = httpContext.Request.Query["title"];
            Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();

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
                    CommentId = comment.Id.ToString(),
                    ParentId = comment.ParentId.ToString(),
                    Author = comment.Author.Username,
                    Avatar = avatar,
                    Date = comment.DateTime.ToString(DatePattern),
                    Time = comment.DateTime.ToString(TimePattern),
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
            User user = await repository.Users.GetByIdAsync(userId);
            if (!(user?.Role?.WriteComments ?? false))
                return;

            //var httpContext = Context.GetHttpContext();
            //string link = httpContext.Request.Query["title"];
            Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();

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
                            CommentId = comment.Id.ToString(),
                            Author = comment.Author.Username,
                            Avatar = avatar,
                            Date = comment.DateTime.ToString(DatePattern),
                            Time = comment.DateTime.ToString(TimePattern),
                            UpdatedDate = comment.UpdatedDateTime?.ToString(DatePattern),
                            UpdatedTime = comment.UpdatedDateTime?.ToString(TimePattern),
                            Text = comment.Text
                        };
                        await Clients.All.SendAsync("UpdatedComment", updatedComment);
                    }
                }
            }
        }

        public async Task DeleteComment(string title, string commentId)
        {
            var httpContext = Context.GetHttpContext();
            //string link = httpContext.Request.Query["title"];
            Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();

            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await repository.Users.GetByIdAsync(userId);

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
                        CommentId = comment.Id.ToString(),
                        Author = comment.Author.Username,
                        Avatar = avatar,
                        Date = comment.DateTime.ToString("yyyy/MM/dd"),
                        Time = comment.DateTime.ToString("HH/mm"),
                        UpdatedDate = comment.UpdatedDateTime?.ToString("yyyy/MM/dd"),
                        UpdatedTime = comment.UpdatedDateTime?.ToString("HH/mm"),
                        Text = comment.Text
                    };
                    await Clients.All.SendAsync("DeletedComment", deletedComment);
                }
            }
        }

        ///// <summary>
        ///// Deletes a comment and child comments from the database. Requires more advanced role
        ///// </summary>
        ///// <param name="commentId">Id of the comment to delete</param>
        ///// <returns></returns>
        //public async Task DeleteWithBranch(string commentId)
        //{
        //    Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
        //    User user = await repository.Users.GetByIdAsync(userId);
        //    if (!((user?.Role as ExtendedRole)?.ModerateComments ?? default))
        //        return;

        //    if (Guid.TryParse(commentId, out Guid commentGuid))
        //    {
        //        Comment comment = await repository.Comments.GetByIdAsync(commentGuid);
        //        for (int i = 0; i < comment.Children.Count; i++)
        //        {
        //            Comment childComment = comment.Children[i];
        //            await repository.Comments.DeleteAsync(childComment.Id);
        //            comment.Children.RemoveAt(i);
        //        }
        //        await repository.Comments.DeleteAsync(comment.Id);


        //    }
        //}

        public async Task LikeArticle(string title)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await repository.Users.GetByIdAsync(userId);
            if (user == null)
                return;

            Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();
            if(article != null)
            {
                if(!await repository.ArticleLikes.ContainsAsync(article.Id, userId))
                {
                    await repository.ArticleLikes.AddForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeAdded");
                }
                else
                {
                    await repository.ArticleLikes.RemoveForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleLikeRemoved");
                }
                int number = repository.ArticleLikes.Count(article.Id);
                await Clients.All.SendAsync("ArticleLikesCounted", article.Link, number);
            }
        }

        public async Task BookmarkArticle(string title)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await repository.Users.GetByIdAsync(userId);
            if (user == null)
                return;

            Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();
            if(article != null)
            {
                if(!await repository.ArticleBookmarks.ContainsAsync(article.Id, userId))
                {
                    await repository.ArticleBookmarks.AddForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkAdded");
                }
                else
                {
                    await repository.ArticleBookmarks.RemoveForAsync(article.Id, userId);
                    await Clients.User(Context.UserIdentifier).SendAsync("ArticleBookmarkRemoved");
                }
                int number = repository.ArticleBookmarks.Count(article.Id);
                await Clients.All.SendAsync("ArticleBookmarksCounted", article.Link, number);
            }
        }

        public async Task LikeComment(string commentId)
        {
            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid userId);
            User user = await repository.Users.GetByIdAsync(userId);
            if (user == null)
                return;

            if(Guid.TryParse(commentId, out Guid commentGuid))
            {
                if((await repository.Comments.GetByIdAsync(commentGuid)) != null)
                {
                    if (!await repository.CommentLikes.ContainsAsync(commentGuid, userId))
                    {
                        await repository.CommentLikes.AddForAsync(commentGuid, userId);
                        await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeAdded", commentId);
                    }
                    else
                    {
                        await repository.CommentLikes.RemoveForAsync(commentGuid, userId);
                        await Clients.User(Context.UserIdentifier).SendAsync("CommentLikeRemoved", commentId);
                    }
                    var number = repository.CommentLikes.Count(commentGuid);
                    await Clients.All.SendAsync("CommentLikesCounted", commentId, number);
                }
            }
        }


        //[AllowAnonymous]
        //public async Task LikesCount(string title, string articleId = null)
        //{
        //    Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();
        //    if(article != null)
        //    {
        //        int number = article.Likes?.Count ?? default;
        //        if(articleId != null)
        //            await Clients.All.SendAsync("LikesCounted", number, articleId);
        //        else
        //            await Clients.All.SendAsync("LikesCounted", number);
        //    }
        //}

        //[AllowAnonymous]
        //public async Task BookmarksCount(string title, string articleId = null)
        //{
        //    Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();
        //    if(article != null)
        //    {
        //        int number = article.Bookmarks?.Count ?? default;
        //        if (articleId != null)
        //            await Clients.All.SendAsync("BookmarksCounted", number, articleId);
        //        else
        //            await Clients.All.SendAsync("BookmarksCounted", number);
        //    }
        //}

        //[AllowAnonymous]
        //public async Task CommentsCount(string title, string articleId = null)
        //{
        //    Article article = repository.Articles.Find(a => a.Link.Equals(title)).FirstOrDefault();
        //    if(article != null)
        //    {
        //        int number = article.Comments?.Count ?? default;
        //        if(articleId != null)
        //            await Clients.All.SendAsync("CommentsCounted", number, articleId);
        //        else
        //            await Clients.All.SendAsync("CommentsCounted", number);
        //    }
        //}
        
        //[AllowAnonymous]
        //public async Task CommentLikesCount(string commentId)
        //{
        //    if(Guid.TryParse(commentId, out Guid commentGuid))
        //    {
        //        var number = repository.CommentLikes.Count(commentGuid);
        //        await Clients.All.SendAsync("CommentLikesCounted", number);
        //    }
        //}
    }
}

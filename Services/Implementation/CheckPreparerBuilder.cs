using Domain.Entities;
using Repo.Interfaces;
using Services.Interfaces;
using System;

namespace Services.Implementation
{
    public class CheckPreparerBuilder
    {
        public IRepository Repository { get; }
        public IUserService UserService { get; }
        public IArticleService ArticleService { get; }
        public ICommentsService CommentsService { get; }

        public CheckPreparerBuilder(IRepository repository,
            IUserService userService,
            IArticleService articleService,
            ICommentsService commentsService)
        {
            Repository = repository;
            UserService = userService;
            ArticleService = articleService;
            CommentsService = commentsService;
        }
        /// <summary>
        /// Creating a ICheckPreparer instance
        /// </summary>
        /// <param name="user">The user with extended role who has access to the list</param>
        /// <param name="queueList">List's type</param>
        /// <returns>ICheckPreparer instance</returns>
        /// <remarks>Necessary to make sure that the user and the list view are present in the ICheckPreparer</remarks>
        public ICheckPreparer Build(User user, string queueList)
        {
            if (user.Role is null || user.Role is not ExtendedRole)
                throw new ArgumentException();
            return new CheckPreparer(this, user, queueList);
        }
    }
}

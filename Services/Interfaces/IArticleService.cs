using Domain.Entities;
using Domain.Entities.Enums;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IArticleService
    {
        /// <summary>
        /// Gets the article.
        /// </summary>
        /// <param name="articleId">The article's id</param>
        /// <returns>The article.</returns>
        Task<Article> GetArticleByIdAsync(Guid articleId);
        /// <summary>
        /// Gets the article.
        /// </summary>
        /// <param name="link">Article's relative link</param>
        /// <returns>The article.</returns>
        Task<Article> GetArticleByLinkAsync(string link);

        /// <summary>
        /// Creates an article using user's Id and ArticleWriteViewModel.
        /// </summary>
        /// <param name="userId">User's identifier</param>
        /// <param name="articleWriteModel">Model that was filled in by the user</param>
        /// <returns>Created article</returns>
        Task<Article> CreateArticleAsync(Guid userId, ArticleWriteViewModel articleWriteModel);

        /// <summary>
        /// Checks the existence of the article for the specified condition
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>True if the article exists, false if not</returns>
        bool HasArticle(Func<Article, bool> predicate);
        Task<Article> FindArticleAsync(Func<Article, bool> predicate);
        /// <summary>
        /// Deletes the article
        /// </summary>
        /// <param name="articleId">Identifier of the article to delete</param>
        /// <returns></returns>
        Task DeleteArticleAsync(Guid articleId);
        /// <summary>
        /// Updates the article
        /// </summary>
        /// <param name="article">Identifier of the article to update</param>
        /// <returns></returns>
        Task UpdateArticleAsync(Article article);
        Task<Article> GetPreparedArticleAsync(Article article);
    }
}

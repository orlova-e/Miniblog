using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IArticleService
    {
        /// <summary>
        /// Gets the article using its link.
        /// </summary>
        /// <param name="link">Article's relative link</param>
        /// <returns>The article.</returns>
        Task<Article> GetArticleByLinkAsync(string link);
        /// <summary>
        /// Creates an article using the ArticleData class
        /// </summary>
        /// <param name="articleData">Article that was filled in by the user</param>
        /// <returns>Created article</returns>
        Task<Article> CreateArticleAsync(ArticleData articleData);
        /// <summary>
        /// Checks the existence of the article for the specified condition
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>True if the article exists, false if not</returns>
        bool HasArticle(Func<Article, bool> predicate);
        /// <summary>
        /// Deletes the article
        /// </summary>
        /// <param name="articleId">Identifier of the article to delete</param>
        /// <returns></returns>
        Task DeleteArticleAsync(Guid articleId);
        /// <summary>
        /// Updates the article
        /// </summary>
        /// <param name="article">Article to update</param>
        /// <param name="articleData">New article's data</param>
        /// <returns></returns>
        Task UpdateArticleAsync(Article article, ArticleData articleData);
    }
}

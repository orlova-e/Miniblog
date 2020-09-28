using Miniblog.Models.Entities;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miniblog.Models.App.Interfaces
{
    public interface IArticlesService
    {
        /// <summary>
        /// Gets the article with all objects in them.
        /// </summary>
        /// <param name="articleId">The article's id</param>
        /// <returns>The article with all objects in them.</returns>
        Task<Article> GetArticleByIdAsync(Guid articleId);
        /// <summary>
        /// Gets the article with all objects in them.
        /// </summary>
        /// <param name="link">Article's relative link</param>
        /// <returns>The article with all objects in them.</returns>
        Task<Article> GetArticleByLinkAsync(string link);
        /// <summary>
        /// Finds and returns articles with all objects in them.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>List of articles with all objects in them.</returns>
        Task<List<Article>> FindArticlesAsync(Func<Article, bool> predicate);
        Task<Article> CreateArticleAsync(Guid userId, ArticleWriteViewModel articleWriteModel);
        bool HasArticle(Func<Article, bool> predicate);
        Task DeleteArticleAsync(Guid articleId);
        Task UpdateArticleAsync(Article article);

        ///// <summary>
        ///// Gets articles with all objects in them.
        ///// </summary>
        ///// <param name="userId">The author's id</param>
        ///// <returns></returns>
        //List<Article> GetArticles(Guid userId);
    }
}

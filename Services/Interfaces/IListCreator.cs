using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IListCreator
    {
        /// <summary>
        /// Finds and returns entries without any additional filters.
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>List of entries.</returns>
        List<Article> FindEntries(Func<Article, bool> predicate);

        /// <summary>
        /// Finds and returns visible articles.
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>List of articles.</returns>
        Task<List<Article>> FindArticlesAsync(Func<Article, bool> predicate);

        /// <summary>
        /// Returns visible user's favourite articles
        /// </summary>
        /// <param name="userId">User's identifier</param>
        /// <returns>Visible user's favourite articles</returns>
        Task<List<Article>> GetFavouritesAsync(Guid userId);

        /// <summary>
        /// Returns visible user's bookmarked articles
        /// </summary>
        /// <param name="userId">User's identifier</param>
        /// <returns>Visible user's bookmarked articles</returns>
        Task<List<Article>> GetBookmarkedAsync(Guid userId);

        /// <summary>
        /// Returns visible user's commented articles
        /// </summary>
        /// <param name="userId">User's identifier</param>
        /// <returns>Visible user's commented articles</returns>
        Task<List<Article>> GetCommentedAsync(Guid userId);

        /// <summary>
        /// Finds and returns invisible articles.
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>List of drafts.</returns>
        List<Article> FindDrafts(Guid userId);
    }
}

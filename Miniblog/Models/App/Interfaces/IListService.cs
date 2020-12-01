using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miniblog.Models.App.Interfaces
{
    public interface IListService
    {
        /// <summary>
        /// Finds and returns visible articles.
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>List of articles.</returns>
        Task<List<Article>> FindArticlesAsync(Func<Article, bool> predicate);
        /// <summary>
        /// Finds and returns invisible articles.
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>List of drafts.</returns>
        List<Article> FindDrafts(Guid userId);
        /// <summary>
        /// Finds and returns entries without any additional filters.
        /// </summary>
        /// <param name="predicate">Search condition</param>
        /// <returns>List of entries.</returns>
        List<Article> FindEntries(Func<Article, bool> predicate);
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
        /// Returns ListSortingType enumeration
        /// </summary>
        /// <param name="sorting">String representation of ListSortingType value</param>
        /// <returns>ListSortingType enumeration</returns>
        ListSorting GetSortingType(string sorting = "newfirst");
        /// <summary>
        /// Sorts the existing list of articles using the ListSortingType enumeration.
        /// </summary>
        /// <param name="articles">List of articles</param>
        /// <param name="sortingType">ListSortingType enumeration value</param>
        /// <returns>Sorted list.</returns>
        List<Article> SortList(List<Article> articles, ListSorting sortingType = ListSorting.NewFirst);
        /// <summary>
        /// Returns a selection of articles using the page number and sorting type.
        /// If the list of articles is empty, the method tries to select public articles.
        /// </summary>
        /// <param name="start">Current page number</param>
        /// <param name="sortingType">ListSortingType enumeration value</param>
        /// <param name="articles">List of articles</param>
        /// <returns>The selection of articles.</returns>
        List<Article> GetSelection(List<Article> articles, uint start = 1, ListSorting sortingType = ListSorting.NewFirst);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">Number of current page for which articles are being searched</param>
        /// <param name="sortingType">ListSortingType enumeration value</param>
        /// <returns>The ListViewModel instance.</returns>
        ListViewModel GetListModel(List<Article> articles, uint start = 1, string sorting = "newfirst");

        //Task<List<Article>> GetArticlesAsync();
        //Task<List<Article>> GetVisibleSelectionAsync(int start = 1, ListSortingType sortingType = ListSortingType.NewFirst, List<Article> articles = null);
    }
}

using Domain.Entities;
using Domain.Entities.Enums;
using System.Collections.Generic;
using Web.ViewModels;

namespace Web.App.Interfaces
{
    public interface IListPreparer
    {
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
        /// <param name="articles">Articles to prepare</param>
        /// <param name="start">Number of current page for which articles are being searched</param>
        /// <param name="listSorting">ListSortingType enumeration value</param>
        /// <returns></returns>
        ListViewModel GetListModel(List<Article> articles, uint start = 1, ListSorting listSorting = ListSorting.NewFirst);
    }
}

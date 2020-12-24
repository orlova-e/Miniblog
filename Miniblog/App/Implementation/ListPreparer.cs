using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.Extensions.Options;
using Miniblog.Configuration;
using Miniblog.ViewModels;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Implementation
{
    public class ListPreparer : IListPreparer
    {
        public BlogOptions BlogOptions { get; private set; }
        public ListPreparer(IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            BlogOptions = optionsSnapshot.Value;
        }

        public ListSorting GetSortingType(string sortby = "newfirst")
        {
            if (!Enum.TryParse(sortby.ToLower().Replace(' ', '\0'), true, out ListSorting sortingType))
                sortingType = ListSorting.NewFirst;
            return sortingType;
        }

        public List<Article> SortList(List<Article> articles, ListSorting sortingType = ListSorting.NewFirst)
        {
            if (articles == null)
                throw new ArgumentNullException();
            else if (!articles.Any())
                throw new ArgumentException();

            IOrderedEnumerable<Article> sortedArticles;

            switch (sortingType)
            {
                case ListSorting.NewFirst:
                    sortedArticles = articles
                        .OrderByDescending(a => a.DateTime);
                    break;
                case ListSorting.OldFirst:
                    sortedArticles = articles
                        .OrderBy(a => a.DateTime);
                    break;
                case ListSorting.Alphabetically:
                    sortedArticles = articles
                        .OrderBy(a => a.Header);
                    break;
                case ListSorting.AlphabeticallyDescending:
                    sortedArticles = articles
                        .OrderByDescending(a => a.Header);
                    break;
                case ListSorting.MostLiked:
                    sortedArticles = articles
                        .OrderBy(a => a.Likes.Count);
                    break;
                default:
                    goto case ListSorting.NewFirst;
            }
            return sortedArticles.ToList();
        }

        public List<Article> GetSelection(List<Article> articles, uint start = 1, ListSorting sortingType = ListSorting.NewFirst)
        {
            if (articles == null)
                throw new ArgumentNullException();
            else if (!articles.Any())
                throw new ArgumentException();

            ListOptions listOptions = BlogOptions.ListOptions;
            if (listOptions.OverrideForUserArticle)
                sortingType = listOptions.ListSortingDefaultType;

            articles = SortList(articles, sortingType);

            if (start > 0
                && listOptions.ArticlesPerPage.Value > 0 
                && --start * listOptions.ArticlesPerPage.Value + listOptions.ArticlesPerPage.Value <= articles.Count)
            {
                articles = articles
                    .Skip((int)start * listOptions.ArticlesPerPage.Value)
                    .Take(listOptions.ArticlesPerPage.Value)
                    .ToList();
            }
            else if (++start <= 0 && listOptions.ArticlesPerPage.Value > 0 && listOptions.ArticlesPerPage.Value <= articles.Count)
            {
                articles = articles
                    .Take(listOptions.ArticlesPerPage.Value)
                    .ToList();
            }
            else if (listOptions.ArticlesPerPage.Value > 0 && listOptions.ArticlesPerPage.Value <= articles.Count)
            {
                articles = articles
                    .TakeLast(listOptions.ArticlesPerPage.Value)
                    .ToList();
            }

            return articles;
        }

        public ListViewModel GetListModel(List<Article> articles, uint start = 1, string sorting = "newfirst")
        {
            ListSorting sortingType = GetSortingType(sorting);
            ListOptions listOptions = BlogOptions.ListOptions;

            double total = (double)articles.Count / (double)listOptions.ArticlesPerPage.Value;
            total = Math.Ceiling(total);

            if(articles != null && articles.Any())
                articles = GetSelection(articles, start, sortingType);

            ListViewModel listViewModel = new ListViewModel
            {
                CurrentPageNumber = start,
                TotalPages = (int)total,
                ListSortingType = sortingType,
                Articles = articles
            };
            return listViewModel;
        }
    }
}

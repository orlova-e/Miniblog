using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.App.Interfaces;
using Web.Configuration;
using Web.ViewModels;

namespace Web.App.Implementation
{
    public class ListPreparer : IListPreparer
    {
        public BlogOptions BlogOptions { get; private set; }
        public ListPreparer(IOptionsSnapshot<BlogOptions> optionsSnapshot)
        {
            BlogOptions = optionsSnapshot.Value;
        }

        public List<Article> SortList(List<Article> articles, ListSorting sortingType = ListSorting.NewFirst)
        {
            IOrderedEnumerable<Article> sortedArticles = sortingType switch
            {
                ListSorting.MostLiked => articles.OrderByDescending(a => a.Likes.Count),
                ListSorting.OldFirst => articles.OrderBy(a => a.DateTime),
                ListSorting.Alphabetically => articles.OrderBy(a => a.Header),
                ListSorting.AlphabeticallyDescending => articles.OrderByDescending(a => a.Header),
                _ => articles.OrderByDescending(a => a.DateTime),
            };

            return sortedArticles.ToList();
        }

        public List<Article> GetSelection(List<Article> articles, uint start = 1, ListSorting sortingType = ListSorting.NewFirst)
        {
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

        public ListViewModel GetListModel(List<Article> articles, uint start = 1, ListSorting listSorting = ListSorting.NewFirst)
        {
            ListOptions listOptions = BlogOptions.ListOptions;

            double total = (double)articles.Count / (double)listOptions.ArticlesPerPage.Value;
            total = Math.Ceiling(total);

            if (!listOptions.OverrideForUserArticle)
            {
                for (int i = 0; i < articles.Count; i++)
                {
                    articles[i].DisplayOptions = (ArticleOptions)listOptions;
                    articles[i].DisplayOptions.ColorTheme = BlogOptions.WebsiteOptions.ColorTheme;
                }
            }

            if (articles != null && articles.Any())
                articles = GetSelection(articles, start, listSorting);

            ListViewModel listViewModel = new ListViewModel
            {
                CurrentPageNumber = start,
                TotalPages = (int)total,
                ListSortingType = listSorting,
                Articles = articles
            };
            return listViewModel;
        }
    }
}

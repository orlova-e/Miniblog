using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Configuration;

namespace Web.ViewModels
{
    public class ListViewModel
    {
        public string PageName { get; set; }
        public uint Current { get; private set; }
        public int Total { get; private set; }
        public bool HasPrevious => Current > 1;
        public bool HasNext => Current > Total;
        public ListSorting ListSorting { get; private set; }
        public List<Article> Articles { get; private set; }

        public ListViewModel() { }
        public ListViewModel(int current,
            List<Article> articles,
            ListOptions listOptions,
            ListSorting listSorting = ListSorting.NewFirst)
        {
            Current = (uint)Math.Abs(current);
            Total = (int)Math.Ceiling(articles.Count / (double)listOptions.ArticlesPerPage.Value);
            ListSorting = listSorting;

            if (articles is not null)
            {
                var sortedArticles = ListSorting switch
                {
                    ListSorting.MostLiked => articles.OrderByDescending(a => a.Likes.Count),
                    ListSorting.OldFirst => articles.OrderBy(a => a.DateTime),
                    ListSorting.Alphabetically => articles.OrderBy(a => a.Header),
                    ListSorting.AlphabeticallyDescending => articles.OrderByDescending(a => a.Header),
                    _ => articles.OrderByDescending(a => a.DateTime),
                };

                Articles = sortedArticles
                    .Skip(((int)Current - 1) * listOptions.ArticlesPerPage.Value)
                    .Take(listOptions.ArticlesPerPage.Value)
                    .ToList();
            }
        }
    }
}

using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Configuration;

namespace Web.ViewModels
{
    public class ListViewModel<T> where T : Entity
    {
        public string PageName { get; set; }
        public string ItemName { get; set; }
        public uint Current { get; private set; }
        public int Total { get; private set; }
        public bool HasPrevious => Current > 1;
        public bool HasNext => Current < Total && Total > 0;
        public ListSorting ListSorting { get; private set; }
        public List<T> Entities { get; private set; }

        public ListViewModel() { }
        public ListViewModel(uint current,
            List<T> entities,
            ListOptions listOptions,
            ListSorting listSorting = ListSorting.NewFirst)
        {
            Current = current;
            Total = (int)Math.Round(entities.Count / (double)listOptions.ArticlesPerPage.Value, MidpointRounding.ToPositiveInfinity);
            ListSorting = listSorting;

            if (entities is List<Article> articles)
            {
                var sortedArticles = ListSorting switch
                {
                    ListSorting.MostLiked => articles.OrderByDescending(a => a.Likes.Count),
                    ListSorting.OldFirst => articles.OrderBy(a => a.DateTime),
                    ListSorting.Alphabetically => articles.OrderBy(a => a.Header),
                    ListSorting.AlphabeticallyDescending => articles.OrderByDescending(a => a.Header),
                    _ => articles.OrderByDescending(a => a.DateTime),
                };
                entities = sortedArticles.ToList() as List<T>;
            }
            else if (entities.Any())
            {
                Type type = entities.GetType().DeclaringType;
                entities = typeof(T).Name switch
                {
                    nameof(Topic) => (entities as List<Topic>).OrderBy(t => t.Name).ToList() as List<T>,
                    nameof(Series) => (entities as List<Series>).OrderBy(s => s.Name).ToList() as List<T>,
                    nameof(User) => (entities as List<User>).OrderBy(u => u.Username).ToList() as List<T>,
                    nameof(Tag) => (entities as List<Tag>).OrderBy(t => t.Name).ToList() as List<T>,
                    _ => throw new NotImplementedException()
                };
            }

            Entities = entities
                .Skip(((int)Current - 1) * listOptions.ArticlesPerPage.Value)
                .Take(listOptions.ArticlesPerPage.Value)
                .ToList() as List<T>;
        }
    }
}

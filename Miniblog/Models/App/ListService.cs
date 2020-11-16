using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miniblog.Models.App.Interfaces;
using Miniblog.Models.App;
using Miniblog.Models.Services.Interfaces;
using Miniblog.Models.Services;
using Miniblog.Models.Entities.Enums;
using Miniblog.Models.Entities;
using Miniblog.ViewModels;

namespace Miniblog.Models.App
{
    public class ListService : IListService
    {
        public IRepository repository { get; private set; }
        public IArticlesService articlesService { get; private set; }
        public ListService(IRepository repository, IArticlesService articlesService)
        {
            this.repository = repository;
            this.articlesService = articlesService;
        }

        public async Task<List<Article>> FindArticlesAsync(Func<Article, bool> predicate)
        {
            List<Article> articles = (await repository.Articles.GetAllAsync())
                 .Where(predicate)
                 .Where(a => a.Visibility && a.EntryType == EntryType.Article)
                 .OrderByDescending(a => a.DateTime.Ticks)
                 .ToList();

            for (int i = 0; i < articles.Count; i++)
            {
                articles[i] = await articlesService.GetFullArticleAsync(articles[i]);
            }
            return articles;
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

        public async Task<List<Article>> GetSelectionAsync(List<Article> articles, uint start = 1, ListSorting sortingType = ListSorting.NewFirst)
        {
            if (articles == null)
                throw new ArgumentNullException();
            else if (!articles.Any())
                throw new ArgumentException();

            ListDisplayOptions listOptions = await repository.ListDisplayOptions.FirstOrDefaultAsync();
            if (listOptions.OverrideForUserArticle)
                sortingType = listOptions.ListSortingDefaultType;

            articles = SortList(articles, sortingType);

            if (start > 0 && listOptions.ArticlesPerPage > 0 && --start * listOptions.ArticlesPerPage + listOptions.ArticlesPerPage <= articles.Count)
            {
                articles = articles
                    .Skip((int)start * listOptions.ArticlesPerPage)
                    .Take(listOptions.ArticlesPerPage)
                    .ToList();
            }
            else if (++start <= 0 && listOptions.ArticlesPerPage > 0 && listOptions.ArticlesPerPage <= articles.Count)
            {
                articles = articles
                    .Take(listOptions.ArticlesPerPage)
                    .ToList();
            }
            else if (listOptions.ArticlesPerPage > 0 && listOptions.ArticlesPerPage <= articles.Count)
            {
                articles = articles
                    .TakeLast(listOptions.ArticlesPerPage)
                    .ToList();
            }

            return articles;
        }

        public List<Article> FindDrafts(Guid userId)
        {
            List<Article> articles = repository.Articles
                .Find(a=>a.UserId == userId)
                .Where(a => !a.Visibility && a.EntryType == EntryType.Article)
                .ToList();
            return articles;
        }

        public List<Article> FindEntries(Func<Article, bool> predicate)
        {
            List<Article> articles = repository.Articles
                .Find(predicate)
                .ToList();
            return articles;
        }

        public async Task<List<Article>> GetFavouritesAsync(Guid userId)
        {
            List<Article> articles = (await repository.ArticleLikes
                .GetAllEntriesForAsync(userId))
                .Where(a => a.Visibility && a.EntryType == EntryType.Article)
                .ToList();
            return articles;
        }

        public async Task<List<Article>> GetBookmarkedAsync(Guid userId)
        {
            List<Article> articles = (await repository.ArticleBookmarks
                .GetAllEntriesForAsync(userId))
                .Where(a => a.Visibility && a.EntryType == EntryType.Article)
                .ToList();
            return articles;
        }

        public async Task<List<Article>> GetCommentedAsync(Guid userId)
        {
            IEnumerable<Guid> articlesId = (await repository.CommentLikes
                .GetAllEntriesForAsync(userId))
                .Select(c => c.ArticleId);

            List<Article> articles = new List<Article>();

            foreach(Guid id in articlesId)
            {
                Article article = await repository.Articles.GetByIdAsync(id);
                if(!articles.Contains(article) && article.Visibility && article.EntryType == EntryType.Article)
                    articles.Add(article);
            }
            return articles;
        }

        public async Task<ListViewModel> GetListModelAsync(List<Article> articles, uint start = 1, string sorting = "newfirst")
        {
            //if (articles == null)
            //    throw new ArgumentNullException(paramName: articles.GetType().Name);
            //else if (!articles.Any())
            //    throw new ArgumentException("There is no articles to create model", articles.GetType().Name);

            ListSorting sortingType = GetSortingType(sorting);
            ListDisplayOptions displayOptions = await repository.ListDisplayOptions.FirstOrDefaultAsync();

            double total = (double)articles.Count / (double)displayOptions.ArticlesPerPage;
            total = Math.Ceiling(total);

            if(articles != null && articles.Any())
                articles = await GetSelectionAsync(articles, start, sortingType);

            ListViewModel listViewModel = new ListViewModel
            {
                CurrentPageNumber = start,
                TotalPages = (int)total,
                ListSortingType = sortingType,
                Articles = articles
            };
            return listViewModel;
        }

        //public Task<List<Article>> GetVisibleSelectionAsync(int start = 1, ListSortingType sortingType = ListSortingType.NewFirst, List<Article> articles = null)
        //{
        //    throw new NotImplementedException();
        //}


    }
}

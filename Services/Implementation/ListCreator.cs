using Domain.Entities;
using Domain.Entities.Enums;
using Repo.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ListCreator : IListCreator
    {
        public IRepository repository { get; private set; }
        public ListCreator(IRepository repository)
        {
            this.repository = repository;
        }

        public List<Article> FindEntries(Func<Article, bool> predicate)
        {
            List<Article> articles = repository.Articles
                .Find(predicate)
                .ToList();
            return articles;
        }

        public async Task<List<Article>> FindArticlesAsync(Func<Article, bool> predicate)
        {
            List<Article> articles = (await repository.Articles.FindAsync(predicate))
                 .Where(a => a.Visibility && a.EntryType == EntryType.Article)
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

            foreach (Guid id in articlesId)
            {
                Article article = await repository.Articles.GetByIdAsync(id);
                if (!articles.Contains(article) && article.Visibility && article.EntryType == EntryType.Article)
                    articles.Add(article);
            }
            return articles;
        }

        public List<Article> FindDrafts(Guid userId)
        {
            List<Article> articles = repository.Articles
                .Find(a => a.UserId == userId)
                .Where(a => !a.Visibility && a.EntryType == EntryType.Article)
                .ToList();
            return articles;
        }
    }
}

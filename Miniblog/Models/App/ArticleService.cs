using Miniblog.Models.App.Interfaces;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Miniblog.Models.App
{
    public class ArticleService : IArticlesService
    {
        public IRepository repository { get; set; }
        public ArticleService(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Article> GetArticleByLinkAsync(string link)
        {
            Article article = repository.Articles.Find(a => a.Link == link).FirstOrDefault();
            if (article != null)
                article = await GetFullArticlAsync(article);
            return article;
        }
        public async Task<Article> GetArticleByIdAsync(Guid articleId)
        {
            Article article = await repository.Articles.GetByIdAsync(articleId);
            if (article != null)
                article = await GetFullArticlAsync(article);
            return article;
        }

        public async Task<Article> CreateArticleAsync(Guid userId, ArticleWriteViewModel articleViewModel)
        {
            User currentUser = await repository.Users.GetByIdAsync(userId);

            Topic topic = null;
            if (articleViewModel.Topic != null)
            {
                topic = repository.Topics.Find(t => t.Name == articleViewModel.Topic).FirstOrDefault();
                if (topic == null && currentUser.Role.CreateTopics)
                {
                    topic = new Topic()
                    {
                        Author = currentUser,
                        Name = articleViewModel.Topic
                    };
                    await repository.Topics.CreateAsync(topic);
                    topic = repository.Topics.Find(t => t.Name == topic.Name).FirstOrDefault();
                }
            }

            //series
            Series series = null;
            if (articleViewModel.Series != null)
            {
                series = repository.Series.Find(s => s.Name == articleViewModel.Series && s.UserId == currentUser.Id).FirstOrDefault();
                if (series == null)
                {
                    string seriesLink = WebUtility.UrlEncode(articleViewModel.Series);
                    var otherSeries = repository.Series.Find(s => s.Link == seriesLink).FirstOrDefault();
                    if (otherSeries != null)
                    {
                        string fromName = seriesLink;
                        int counter = 1;
                        while (seriesLink == otherSeries?.Link)
                        {
                            seriesLink = $"{fromName}-{counter}";
                            otherSeries = repository.Series.Find(s => s.Link == seriesLink).FirstOrDefault();
                            counter++;
                        }
                    }
                    series = new Series()
                    {
                        Name = articleViewModel.Series,
                        User = currentUser,
                        Link = seriesLink
                    };
                    await repository.Series.CreateAsync(series);
                    series = repository.Series.Find(s => s.Link == series.Link).FirstOrDefault();
                }
            }

            //tags?

            string link = WebUtility.UrlEncode(articleViewModel.Header);
            var otherArticle = repository.Articles.Find(a => a.Link == link).FirstOrDefault();
            if (otherArticle != null
                || articleViewModel.Header.Equals("Article", StringComparison.OrdinalIgnoreCase)
                || articleViewModel.Header.Equals("Add", StringComparison.OrdinalIgnoreCase)
                || articleViewModel.Header.Equals("List", StringComparison.OrdinalIgnoreCase))
            {
                int counter = 1;
                link = WebUtility.UrlDecode(link);
                string fromHeader = link;
                while (link == otherArticle?.Link) // substitute NULL
                {
                    link = $"{fromHeader}-{counter}";
                    link = WebUtility.UrlEncode(link);
                    otherArticle = repository.Articles.Find(a => a.Link == link).FirstOrDefault();
                    counter++;
                }
            }

            Article article = new Article()
            {
                User = currentUser,
                Header = articleViewModel.Header,
                Text = articleViewModel.Text,
                Topic = topic ?? null,
                Series = series ?? null,
                Tags = articleViewModel.Tags,
                Link = link,
                Visibility = articleViewModel.Visibility,
                MenuVisibility = articleViewModel.MenuVisibility,
                DateTime = DateTimeOffset.UtcNow,
                EntryType = articleViewModel.EntryType,
                DisplayOptions = articleViewModel.DisplayOptions
            };

            await repository.Articles.CreateAsync(article);

            article = repository.Articles.Find(a => a.Link == article.Link).FirstOrDefault();
            return article;
        }

        public bool HasArticle(Func<Article, bool> predicate)
        {
            var article = repository.Articles.Find(predicate).FirstOrDefault();
            if (article != null)
                return false;
            return true;
        }

        public async Task DeleteArticleAsync(Guid articleId)
        {
            await repository.Articles.DeleteAsync(articleId);
        }

        public async Task UpdateArticleAsync(Article article)
        {
            await repository.Articles.UpdateAsync(article);
        }

        public async Task<List<Article>> FindArticlesAsync(Func<Article, bool> predicate)
        {
            ListDisplayOptions listOptions = await repository.ListDisplayOptions.FirstOrDefaultAsync();

            List<Article> articles = (await repository.Articles.GetAllAsync())
                 .Where(predicate)
                 .OrderByDescending(a => a.DateTime.Ticks)
                 .ToList();

            for (int i = 0; i < articles.Count; i++)
            {
                articles[i] = await GetFullArticlAsync(articles[i]);
            }
            return articles;
        }
        private async Task<Article> GetFullArticlAsync(Article article)
        {
            ListDisplayOptions listOptions = await repository.ListDisplayOptions.FirstOrDefaultAsync();

            if (article.User == null)
                article.User = await repository.Users.GetByIdAsync(article.UserId);

            if (listOptions.OverrideForUserArticle || article.DisplayOptions == null)
            {
                article.DisplayOptions = repository.ArticleOptions.Find(d => d.ArticleId == article.Id).FirstOrDefault();
            }
            else
            {
                article.DisplayOptions = (ArticleOptions)listOptions;
            }

            if (article.TopicId != null && article.Topic == null)
                article.Topic = await repository.Topics.GetByIdAsync((Guid)article.TopicId);

            if (article.SeriesId != null && article.Series == null)
                article.Series = await repository.Series.GetByIdAsync((Guid)article.SeriesId);

            if (!article.Comments.Any())
            {
                article.Comments = repository.Comments.Find(c => c.ArticleId == article.Id).ToList();
                for (int i = 0; i < article.Comments.Count; i++)
                {
                    article.Comments[i].Likes = await repository.CommentLikes.GetAsync(article.Comments[i].Id);
                }
            }

            if (!article.Bookmarks.Any())
                article.Bookmarks = await repository.ArticleBookmarks.GetAsync(article.Id);

            if (!article.Likes.Any())
                article.Likes = await repository.ArticleLikes.GetAsync(article.Id);

            return article;
        }
    }
}

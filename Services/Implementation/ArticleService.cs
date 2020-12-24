using Microsoft.Extensions.Options;
using Miniblog.Configuration;
using Services.Interfaces;
using Domain.Entities;
using Repo.Interfaces;
using Miniblog.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ArticleService : IArticlesService
    {
        public IRepository repository { get; private set; }
        public IUserService userService { get; private set; }
        public ArticleService(IRepository repository,
            IUserService userService)
        {
            this.repository = repository;
            this.userService = userService;
        }
        
        public async Task<Article> GetArticleByLinkAsync(string link)
        {
            Article article = repository.Articles.Find(a => a.Link == link).FirstOrDefault();
            if (article != null)
                article = await GetPreparedArticleAsync(article);
            return article;
        }
        
        public async Task<Article> GetArticleByIdAsync(Guid articleId)
        {
            Article article = await repository.Articles.GetByIdAsync(articleId);
            if (article != null)
                article = await GetPreparedArticleAsync(article);
            return article;
        }
        
        public async Task<Article> CreateArticleAsync(Guid userId, ArticleWriteViewModel articleViewModel)
        {
            User currentUser = await userService.GetFromDbAsync(userId);

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

            string[] tags = articleViewModel.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < tags.Length; i++)
            {
                tags[i] = tags[i].Trim();
            }
            string tagsStr = string.Join(',', tags);

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
                Topic = topic,
                Series = series,
                Tags = tagsStr,
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
            if (article == null)
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
        
        public async Task<Article> GetPreparedArticleAsync(Article article)
        {

            if (article.User == null)
                article.User = await userService.GetFromDbAsync(article.UserId);

            if (article.DisplayOptions == null)
            {
                article.DisplayOptions = repository.ArticleOptions.Find(d => d.ArticleId == article.Id).FirstOrDefault();
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

        public async Task<Article> FindArticleAsync(Func<Article, bool> predicate)
        {
            Article article = repository.Articles.Find(predicate).FirstOrDefault();
            if (article != null)
                article = await GetPreparedArticleAsync(article);
            return article;
        }
    }
}

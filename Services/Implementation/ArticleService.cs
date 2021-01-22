using Domain.Entities;
using Repo.Interfaces;
using Services.IndexedValues;
using Services.Interfaces;
using Services.Interfaces.Indexing;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ArticleService : IArticleService
    {
        public IRepository Repository { get; private set; }
        public IUserService UserService { get; private set; }
        public IIndexedObjectsObserver IndexedObjectsObserver { get; }
        public ArticleService(IRepository repository,
            IUserService userService,
            IIndexedObjectsObserver indexedObjectsObserver)
        {
            Repository = repository;
            UserService = userService;
            IndexedObjectsObserver = indexedObjectsObserver;
        }

        public async Task<Article> GetArticleByLinkAsync(string link)
        {
            Article article = Repository.Articles.Find(a => a.Link == link).FirstOrDefault();
            if (article != null)
                article = await GetPreparedArticleAsync(article);
            return article;
        }

        public async Task<Article> GetArticleByIdAsync(Guid articleId)
        {
            Article article = await Repository.Articles.GetByIdAsync(articleId);
            if (article != null)
                article = await GetPreparedArticleAsync(article);
            return article;
        }

        public async Task<Article> CreateArticleAsync(Guid userId, NewArticle articleViewModel)
        {
            User currentUser = await UserService.GetFromDbAsync(userId);

            Topic topic = null;
            if (articleViewModel.Topic != null)
            {
                topic = Repository.Topics.Find(t => t.Name == articleViewModel.Topic).FirstOrDefault();
                if (topic == null && currentUser.Role.CreateTopics)
                {
                    topic = new Topic()
                    {
                        Author = currentUser,
                        Name = articleViewModel.Topic
                    };
                    await Repository.Topics.CreateAsync(topic);
                    topic = Repository.Topics.Find(t => t.Name == topic.Name).FirstOrDefault();
                }
            }

            Series series = null;
            if (articleViewModel.Series != null)
            {
                series = Repository.Series.Find(s => s.Name == articleViewModel.Series && s.UserId == currentUser.Id).FirstOrDefault();
                if (series == null)
                {
                    string seriesLink = WebUtility.UrlEncode(articleViewModel.Series);
                    var otherSeries = Repository.Series.Find(s => s.Link == seriesLink).FirstOrDefault();
                    if (otherSeries != null)
                    {
                        string fromName = seriesLink;
                        int counter = 1;
                        while (seriesLink == otherSeries?.Link)
                        {
                            seriesLink = $"{fromName}-{counter}";
                            otherSeries = Repository.Series.Find(s => s.Link == seriesLink).FirstOrDefault();
                            counter++;
                        }
                    }
                    series = new Series()
                    {
                        Name = articleViewModel.Series,
                        User = currentUser,
                        Link = seriesLink
                    };
                    await Repository.Series.CreateAsync(series);
                    series = Repository.Series.Find(s => s.Link == series.Link).FirstOrDefault();
                }
            }

            string[] tags = articleViewModel.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tags.Length; i++)
            {
                tags[i] = tags[i].Trim();
            }
            string tagsStr = string.Join(',', tags);

            string link = WebUtility.UrlEncode(articleViewModel.Header);
            var otherArticle = Repository.Articles.Find(a => a.Link == link).FirstOrDefault();
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
                    otherArticle = Repository.Articles.Find(a => a.Link == link).FirstOrDefault();
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

            await Repository.Articles.CreateAsync(article);

            article = Repository.Articles.Find(a => a.Link == article.Link).FirstOrDefault();
            await IndexedObjectsObserver.OnNewEntityAsync((ArticleIndexedValues)article);
            return article;
        }

        public bool HasArticle(Func<Article, bool> predicate)
        {
            var article = Repository.Articles.Find(predicate).FirstOrDefault();
            if (article == null)
                return false;
            return true;
        }

        public async Task DeleteArticleAsync(Guid articleId)
        {
            Article article = await Repository.Articles.GetByIdAsync(articleId);
            await Repository.Articles.DeleteAsync(articleId);
            await IndexedObjectsObserver.OnDeletedEntityAsync((ArticleIndexedValues)article);
        }

        public async Task UpdateArticleAsync(Article article)
        {
            await Repository.Articles.UpdateAsync(article);
            Article updated = await Repository.Articles.GetByIdAsync(article.Id);
            await IndexedObjectsObserver.OnUpdatedEntityAsync((ArticleIndexedValues)updated);
        }

        public async Task<Article> GetPreparedArticleAsync(Article article)
        {

            if (article.User == null)
                article.User = await UserService.GetFromDbAsync(article.UserId);

            if (article.DisplayOptions == null)
            {
                article.DisplayOptions = Repository.ArticleOptions.Find(d => d.ArticleId == article.Id).FirstOrDefault();
            }

            if (article.TopicId != null && article.Topic == null)
                article.Topic = await Repository.Topics.GetByIdAsync((Guid)article.TopicId);

            if (article.SeriesId != null && article.Series == null)
                article.Series = await Repository.Series.GetByIdAsync((Guid)article.SeriesId);

            if (!article.Comments.Any())
            {
                article.Comments = Repository.Comments.Find(c => c.ArticleId == article.Id).ToList();
                for (int i = 0; i < article.Comments.Count; i++)
                {
                    article.Comments[i].Likes = await Repository.CommentLikes.GetAsync(article.Comments[i].Id);
                }
            }

            if (!article.Bookmarks.Any())
                article.Bookmarks = await Repository.ArticleBookmarks.GetAsync(article.Id);

            if (!article.Likes.Any())
                article.Likes = await Repository.ArticleLikes.GetAsync(article.Id);

            return article;
        }

        public async Task<Article> FindArticleAsync(Func<Article, bool> predicate)
        {
            Article article = Repository.Articles.Find(predicate).FirstOrDefault();
            if (article != null)
                article = await GetPreparedArticleAsync(article);
            return article;
        }
    }
}

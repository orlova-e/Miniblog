using Domain.Entities;
using Repo.Interfaces;
using Services.IndexedValues;
using Services.Interfaces;
using Services.Interfaces.Indexing;
using System;
using System.Collections.Generic;
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

        public Dictionary<string, string> CheckBeforeCreation(NewArticle newArticle)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            Role role = newArticle.User.Role;
            if (!role.WriteArticles)
                errors.Add("", "Cannot write articles");

            if(!string.IsNullOrWhiteSpace(newArticle.Topic))
            {
                Topic topic = Repository.Topics.Find(t => t.Equals(newArticle.Topic)).FirstOrDefault();
                if (topic is object && !role.CreateTopics)
                    errors.Add("Topic", "Cannot create topics");
            }

            if (newArticle.DisplayOptions != default && !role.OverrideOwnArticle)
                errors.Add("DisplayOptions", "Cannot override own articles");

            return errors;
        }

        public async Task<Article> CreateArticleAsync(NewArticle newArticle)
        {
            User currentUser = newArticle.User;

            Topic topic = null;
            if (newArticle.Topic != null)
            {
                topic = Repository.Topics.Find(t => t.Name == newArticle.Topic).FirstOrDefault();
                if (topic == null && currentUser.Role.CreateTopics)
                {
                    topic = new Topic()
                    {
                        Author = currentUser,
                        Name = newArticle.Topic
                    };
                    await Repository.Topics.CreateAsync(topic);
                    topic = Repository.Topics.Find(t => t.Name == topic.Name).FirstOrDefault();
                }
            }

            Series series = null;
            if (newArticle.Series != null)
            {
                series = Repository.Series.Find(s => s.Name == newArticle.Series && s.UserId == currentUser.Id).FirstOrDefault();
                if (series == null)
                {
                    string seriesLink = WebUtility.UrlEncode(newArticle.Series);
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
                        Name = newArticle.Series,
                        User = currentUser,
                        Link = seriesLink
                    };
                    await Repository.Series.CreateAsync(series);
                    series = Repository.Series.Find(s => s.Link == series.Link).FirstOrDefault();
                }
            }

            string[] tags = newArticle.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Array.ForEach(tags, t =>
            {
                t.Trim();
            });
            List<ArticleTag> articleTags = new List<ArticleTag>(tags.Length);
            IEnumerable<Tag> tagsCollection = Repository.Tags.FindRange(tags);
            string[] existingTags = tagsCollection?.Select(t => t.Name)?.ToArray();
            string[] uncreatedTags = new string[] { };
            if(existingTags is object)
                uncreatedTags = tags.Except(existingTags).ToArray();
            foreach(string uncreated in uncreatedTags)
            {
                Tag tag = new Tag { Name = uncreated, Author = currentUser };
                articleTags.Add(new ArticleTag { Tag = tag });
            }

            string link = WebUtility.UrlEncode(newArticle.Header);
            var otherArticle = Repository.Articles.Find(a => a.Link == link).FirstOrDefault();
            if (otherArticle != null
                || newArticle.Header.Equals("Article", StringComparison.OrdinalIgnoreCase)
                || newArticle.Header.Equals("Add", StringComparison.OrdinalIgnoreCase)
                || newArticle.Header.Equals("List", StringComparison.OrdinalIgnoreCase))
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
                Header = newArticle.Header,
                Text = newArticle.Text,
                Topic = topic,
                Series = series,
                ArticleTags = articleTags,
                Link = link,
                Visibility = newArticle.Visibility,
                MenuVisibility = newArticle.MenuVisibility,
                DateTime = DateTimeOffset.UtcNow,
                EntryType = newArticle.EntryType,
                DisplayOptions = newArticle.DisplayOptions
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

using Domain.Entities;
using Repo.Interfaces;
using Services.Interfaces;
using Services.VisibleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class ArticleService : IArticleService
    {
        private IRepository Repository { get; }
        private IEntityObserver EntityObserver { get; }
        public ArticleService(IRepository repository,
            IEntityObserver entityObserver)
        {
            Repository = repository;
            EntityObserver = entityObserver;
        }

        public async Task<Article> GetArticleByLinkAsync(string link)
        {
            Article article = (await Repository.Articles.FindAsync(a => a.Link == link)).FirstOrDefault();
            return article;
        }

        public Dictionary<string, string> CheckBeforeCreation(ArticleData articleData)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            Role role = articleData.User.Role;
            if (!role.WriteArticles)
                errors.Add("", "Cannot write articles");

            if (!string.IsNullOrWhiteSpace(articleData.Topic))
            {
                Topic topic = Repository.Topics.Find(t => t.Equals(articleData.Topic)).FirstOrDefault();
                if (topic is object && !role.CreateTopics)
                    errors.Add("Topic", "Cannot create topics");
            }

            if (articleData.DisplayOptions != default && !role.OverrideOwnArticle)
                errors.Add("DisplayOptions", "Cannot override own articles");

            return errors;
        }

        public async Task<Article> CreateArticleAsync(ArticleData articleData)
        {
            Article article = new ArticleBuilder(Repository)
                .Header(articleData.Header)
                .Text(articleData.Text)
                .Visibility(articleData.Visibility)
                .User(articleData.User)
                .Topic(articleData.Topic)
                .Series(articleData.Series)
                .Tags(articleData.Tags)
                .MenuVisibility(articleData.MenuVisibility)
                .DisplayOptions(articleData.DisplayOptions)
                .TypeOfEntry(articleData.EntryType)
                .Build();

            await Repository.Articles.CreateAsync(article);
            article = Repository.Articles.Find(a => a.Link == article.Link).FirstOrDefault();
            await EntityObserver.OnNewEntryAsync((VisibleArticleValues)article);
            return article;
        }

        public bool HasArticle(Func<Article, bool> predicate)
        {
            var article = Repository.Articles.Find(predicate).FirstOrDefault();
            if (article is null)
                return false;
            return true;
        }

        public async Task DeleteArticleAsync(Guid articleId)
        {
            Article article = await Repository.Articles.GetByIdAsync(articleId);
            if (article is not null)
            {
                await EntityObserver.OnDeleteAsync((VisibleArticleValues)article);
                await Repository.Articles.DeleteAsync(article.Id);
            }
        }

        public async Task UpdateArticleAsync(Article article, ArticleData articleData)
        {
            Article updated = new ArticleBuilder(Repository, article)
                .Header(articleData.Header)
                .Text(articleData.Text)
                .Visibility(articleData.Visibility)
                .User(articleData.User)
                .Topic(articleData.Topic)
                .Series(articleData.Series)
                .Tags(articleData.Tags)
                .MenuVisibility(articleData.MenuVisibility)
                .DisplayOptions(articleData.DisplayOptions)
                .TypeOfEntry(articleData.EntryType)
                .Build();

            await Repository.Articles.UpdateAsync(updated);
            updated = await Repository.Articles.GetByIdAsync(article.Id);
            await EntityObserver.OnUpdateAsync((VisibleArticleValues)updated);
        }
    }
}

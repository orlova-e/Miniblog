using Domain.Entities;
using Repo.Interfaces;
using System;
using System.Linq;
using System.Net;

namespace Services.Implementation
{
    internal sealed class ArticleBuilder
    {
        private Article _article = new Article();
        private IRepository Repository { get; }
        public ArticleBuilder(IRepository repository)
        {
            Repository = repository;
            _article.DateTime = DateTime.Now;
            _article.MenuVisibility = false;
        }

        public ArticleBuilder(IRepository repository, Article fromArticle)
        {
            Repository = repository;
            _article = fromArticle;
        }

        public ArticleExtendedBuilder User(User user)
        {
            if (_article.User == null)
                _article.User = user;

            if (string.IsNullOrWhiteSpace(_article.Header))
                throw new InvalidOperationException();

            if (string.IsNullOrWhiteSpace(_article.Text))
                throw new InvalidOperationException();

            return new ArticleExtendedBuilder(Repository, _article);
        }

        public ArticleBuilder Header(string header)
        {
            if (_article.Header?.Equals(header) ?? default)
                return this;
            _article.Header = header;
            var builder = Link(header);
            return builder;
        }

        public ArticleBuilder Text(string text)
        {
            _article.Text = text;
            return this;
        }

        public ArticleBuilder Visibility(bool visibility)
        {
            _article.Visibility = visibility;
            return this;
        }

        internal ArticleBuilder Link(string header)
        {
            string link = WebUtility.UrlEncode(header);
            var otherArticle = Repository.Articles.Find(a => a.Link == link).FirstOrDefault();
            if (otherArticle != null
                || header.Equals("Article", StringComparison.OrdinalIgnoreCase)
                || header.Equals("Add", StringComparison.OrdinalIgnoreCase)
                || header.Equals("List", StringComparison.OrdinalIgnoreCase))
            {
                int counter = 1;
                link = WebUtility.UrlDecode(link);
                string fromHeader = link;
                while (link == otherArticle?.Link)
                {
                    link = $"{fromHeader}-{counter}";
                    link = WebUtility.UrlEncode(link);
                    otherArticle = Repository.Articles.Find(a => a.Link == link).FirstOrDefault();
                    counter++;
                }
            }

            _article.Link = link;
            return this;
        }
    }
}

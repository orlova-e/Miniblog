using Domain.Entities;
using Domain.Entities.Enums;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Services.Implementation
{
    internal sealed class ArticleExtendedBuilder
    {
        private Article _article;
        private IRepository Repository { get; }
        internal ArticleExtendedBuilder(IRepository repository, Article article)
        {
            Repository = repository;
            _article = article;
        }

        public ArticleExtendedBuilder MenuVisibility(bool visibility)
        {
            if (visibility && _article.User.Role is ExtendedRole extended && extended.OverrideMenu)
            {
                _article.MenuVisibility = visibility;
            }
            return this;
        }

        public ArticleExtendedBuilder TypeOfEntry(EntryType entryType)
        {
            if (entryType == EntryType.Page && _article.User.Role.Type == RoleType.Administrator)
            {
                _article.EntryType = entryType;
            }
            return this;
        }

        public ArticleExtendedBuilder DisplayOptions(ArticleOptions articleOptions)
        {
            if (_article.User.Role.OverrideOwnArticle)
            {
                _article.DisplayOptions = articleOptions;
            }
            return this;
        }

        public ArticleExtendedBuilder Topic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic) || topic.Equals(_article.Topic?.Name))
                return this;

            Topic articleTopic = Repository.Topics.Find(t => t.Name == topic).FirstOrDefault();
            if (articleTopic is null && _article.User.Role.CreateTopics)
            {
                articleTopic = new Topic()
                {
                    Name = topic
                };
            }

            _article.Topic = articleTopic;
            return this;
        }

        public ArticleExtendedBuilder Series(string series)
        {
            if (string.IsNullOrWhiteSpace(series) || series.Equals(_article.Series?.Name))
                return this;

            string seriesLink = WebUtility.UrlEncode(series);

            bool anotherUserSeries(Series s) => s.Link == seriesLink && s.UserId != _article.User.Id;
            Series otherSeries = Repository.Series.Find(anotherUserSeries).FirstOrDefault();

            int counter = 1;
            string link = seriesLink;
            while (otherSeries is not null)
            {
                string fromName = seriesLink;
                seriesLink = $"{link}-{counter}";
                otherSeries = Repository.Series.Find(anotherUserSeries).FirstOrDefault();
                counter++;
            }

            bool userSeries(Series s) => s.Link == seriesLink && s.UserId == _article.User.Id;
            Series articleSeries = Repository.Series.Find(userSeries).FirstOrDefault();
            if (articleSeries is null)
            {
                articleSeries = new Series()
                {
                    Name = series,
                    User = _article.User,
                    Link = seriesLink
                };
            }

            _article.Series = articleSeries;
            return this;
        }

        public ArticleExtendedBuilder Tags(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags?.Replace(",", "")))
                return this;

            IEnumerable<string> _names = tags
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)?
                .Select(t => t.Trim());

            IEnumerable<Tag> _tags = _names
                .Select(t => new Tag
                {
                    Name = t.Trim()
                });

            IEnumerable<Tag> _exist = Repository.Tags.Find(t => _names.Contains(t.Name));

            if (_exist?.Any() ?? default)
            {
                _article.ArticleTags = _exist.UnionBy(t => t.Name, _tags)
                    .Select(t => new ArticleTag
                    {
                        Article = _article,
                        Tag = t,
                    }).ToList();
            }
            else
            {
                _article.ArticleTags = _tags
                    .Select(t => new ArticleTag
                    {
                        Article = _article,
                        Tag = t
                    }).ToList();
            }

            return this;
        }

        public Article Build()
        {
            return _article;
        }
    }
}

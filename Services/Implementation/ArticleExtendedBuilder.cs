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
            if (visibility && _article.User.Role.Type == RoleType.Administrator)
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
            if (articleTopic == null && _article.User.Role.CreateTopics)
            {
                articleTopic = new Topic()
                {
                    Author = _article.User,
                    Name = topic
                };
                //await Repository.Topics.CreateAsync(topicObj);
                //topic = Repository.Topics.Find(t => t.Name == topic.Name).FirstOrDefault();
            }

            _article.Topic = articleTopic;
            return this;
        }

        public ArticleExtendedBuilder Series(string series)
        {
            if (string.IsNullOrWhiteSpace(series) || series.Equals(_article.Series?.Name))
                return this;

            Series articleSeries = Repository.Series.Find(s => s.Name == series && s.UserId == _article.User.Id).FirstOrDefault();
            if (articleSeries is object)
                return this;

            string seriesLink = WebUtility.UrlEncode(series);
            var otherSeries = Repository.Series.Find(s => s.Link == seriesLink).FirstOrDefault();
            if (otherSeries is object)
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
            articleSeries = new Series()
            {
                Name = series,
                User = _article.User,
                Link = seriesLink
            };
            //await Repository.Series.CreateAsync(articleSeries);
            //series = Repository.Series.Find(s => s.Link == series.Link).FirstOrDefault();

            _article.Series = articleSeries;
            return this;
        }

        public ArticleExtendedBuilder Tags(string tags)
        {
            if (string.IsNullOrWhiteSpace(tags))
                return this;

            string[] oldTags = _article.ArticleTags?
                .Select(t => t.Tag)?
                .Select(t => t.Name)?
                .ToArray();

            string[] tagsArray = tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Array.ForEach(tagsArray, t => { t.Trim(); });

            if (oldTags is object && oldTags.Except(tagsArray).Count() == 0 && tagsArray.Except(oldTags).Count() == 0)
                return this;

            List<ArticleTag> articleTags = new List<ArticleTag>(tagsArray.Length);
            IEnumerable<Tag> tagsCollection = Repository.Tags.Find(t => tagsArray.Contains(t.Name));
            string[] existingTags = tagsCollection?.Select(t => t.Name)?.ToArray();
            string[] uncreatedTags = new string[] { };
            if (existingTags is object)
                uncreatedTags = tagsArray.Except(existingTags).ToArray();
            foreach (string uncreated in uncreatedTags)
            {
                Tag tag = new Tag { Name = uncreated, Author = _article.User };
                articleTags.Add(new ArticleTag { Tag = tag });
            }

            _article.ArticleTags = articleTags;
            return this;
        }

        public Article Build()
        {
            return _article;
        }
    }
}

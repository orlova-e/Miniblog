﻿using Domain.Entities;
using System.Linq;

namespace Services.IndexedValues
{
    public class ArticleIndexedValues : IndexedObject
    {
        public string Header { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public string Tags { get; set; }
        public string Topic { get; set; }
        public string Series { get; set; }

        public static explicit operator ArticleIndexedValues(Article article)
            => new ArticleIndexedValues
            {
                Id = article.Id,
                TypeOfIndexed = article.GetType(),
                Header = article.Header,
                Text = article.Text,
                Author = article.User.Username,
                Tags = string.Join(", ", article.ArticleTags?
                    .Select(t => t.Tag)?
                    .Select(t => t.Name)),
                Topic = article.Topic?.Name,
                Series = article.Series?.Name,
            };
    }
}

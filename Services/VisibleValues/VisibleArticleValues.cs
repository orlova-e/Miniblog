using Domain.Entities;
using System;
using System.Linq;

namespace Services.VisibleValues
{
    public class VisibleArticleValues : VisibleObjectValues
    {
        public string Header { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public string Tags { get; set; }
        public string Topic { get; set; }
        public string Series { get; set; }

        public static explicit operator VisibleArticleValues(Article article)
            => new VisibleArticleValues
            {
                Id = article.Id,
                Header = article.Header,
                Text = article.Text,
                Author = article.User.Username,
                Tags = string.Join(", ", article.ArticleTags?
                    .Select(t => t.Tag)?
                    .Select(t => t.Name)),
                Topic = article.Topic?.Name,
                Series = article.Series?.Name,
            };

        public override int Rate(string propertyName) => propertyName switch
        {
            nameof(Header) => 5,
            nameof(Author) => 5,
            nameof(Series) => 3,
            nameof(Topic) => 2,
            nameof(Tags) => 2,
            nameof(Text) => 1,
            _ => throw new NotImplementedException()
        };
    }
}

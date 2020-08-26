using System;

namespace Miniblog.Models.Entities
{
    public class ArticleTag
    {
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}

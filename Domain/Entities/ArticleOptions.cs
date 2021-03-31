using System;

namespace Domain.Entities
{
    public class ArticleOptions : BaseDisplayOptions
    {
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}

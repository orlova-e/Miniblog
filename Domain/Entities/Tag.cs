using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Tags")]
    public class Tag : Entity
    {
        [Required]
        public string Name { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
        public Tag()
        {
            ArticleTags = new List<ArticleTag>();
        }
    }
}

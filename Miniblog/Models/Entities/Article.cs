using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class Article
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public Guid? TopicId { get; set; }
        public Topic Topic { get; set; }
        public List<ArticleTag> ArticleTags { get; set; }
        public Guid? SeriesId { get; set; }
        public Series Series { get; set; }
        public List<Image> Images { get; set; }
        public List<Comment> Comments { get; set; }
        public UserArticleDisplayOptions UserArticleDisplayOptions { get; set; }
        public Article()
        {
            Images = new List<Image>();
            Comments = new List<Comment>();
            ArticleTags = new List<ArticleTag>();
        }
    }
}

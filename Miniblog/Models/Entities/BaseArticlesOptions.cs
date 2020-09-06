using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public abstract class BaseArticlesOptions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Discriminator { get; }
        public bool Username { get; set; }
        public bool UserIcon { get; set; }
        public bool Date { get; set; }
        public bool Time { get; set; }
        public bool Tags { get; set; }
        public bool Topic { get; set; }
        //public bool ArticlesSeries { get; set; }
        public bool Likes { get; set; }
        public bool Bookmarks { get; set; }
        public bool Comments { get; set; }
    }
}

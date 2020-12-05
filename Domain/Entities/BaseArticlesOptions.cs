using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public abstract class BaseArticlesOptions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Discriminator { get; }
        public bool Username { get; set; }
        [Display(Name = "Date and time")]
        public bool DateAndTime { get; set; }
        public bool Tags { get; set; }
        public bool Topic { get; set; }
        public bool Series { get; set; }
        public bool Likes { get; set; }
        public bool Bookmarks { get; set; }
        public bool Comments { get; set; }
    }
}

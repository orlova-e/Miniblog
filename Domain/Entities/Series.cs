using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Series")]
    public class Series : Entity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Link { get; set; }
        public List<Article> Articles { get; set; }
        public Series()
        {
            Articles = new List<Article>();
        }
    }
}

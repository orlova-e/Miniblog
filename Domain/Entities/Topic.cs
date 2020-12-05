using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Topic
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid? AuthorId { get; set; }
        public User Author { get; set; }
        public List<Article> Articles { get; set; }
        public Topic()
        {
            Articles = new List<Article>();
        }
    }
}

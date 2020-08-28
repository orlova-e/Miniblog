//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Miniblog.Models.Entities
//{
//    public class Tag
//    {
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public Guid Id { get; set; }
//        [Required]
//        public string Name { get; set; }
//        public Guid AuthorId { get; set; }
//        public User Author { get; set; }
//        public List<ArticleTag> ArticleTags { get; set; }
//        public Tag()
//        {
//            ArticleTags = new List<ArticleTag>();
//        }
//    }
//}

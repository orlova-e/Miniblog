//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Miniblog.Models.Entities
//{
//    public class Series
//    {
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public Guid Id { get; set; }
//        [Required]
//        public string Name { get; set; }
//        public List<Article> Articles { get; set; }
//        public Series()
//        {
//            Articles = new List<Article>();
//        }
//    }
//}

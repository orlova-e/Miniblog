using Miniblog.Models.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class Role
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public RoleType Type { get; set; }
        public string Discriminator { get; }
        public bool WriteArticles { get; set; }
        public bool WriteComments { get; set; }
        public bool WriteMessages { get; set; }
        //public bool ReadComments { get; set; }
        public bool OverrideOwnArticle { get; set; }
        public bool CreateTopics { get; set; }
        public bool CreateTags { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}

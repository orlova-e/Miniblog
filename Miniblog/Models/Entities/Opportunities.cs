using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class Opportunities
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
        public string Discriminator { get; }
        public bool WriteArticles { get; set; }
        public bool WriteComments { get; set; }
        public bool WriteMessages { get; set; }
        public bool ReadComments { get; set; }
        public bool CreateTopics { get; set; }
        public bool CreateTags { get; set; }
    }
}
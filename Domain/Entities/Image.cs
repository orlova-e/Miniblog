using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Image
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public int Position { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public byte[] File { get; set; }
        [Required]
        public string FileExtension { get; set; }
    }
}
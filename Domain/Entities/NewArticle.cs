using Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [NotMapped]
    public class NewArticle
    {
        public User User { get; set; }
        [Required(ErrorMessage = "Header is required")]
        [StringLength(200, ErrorMessage = "Header's maximum length is 50 characters")]
        public string Header { get; set; }
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }
        public string Topic { get; set; }
        public string Series { get; set; }
        public string Tags { get; set; }
        public EntryType EntryType { get; set; }
        public bool Visibility { get; set; }
        public bool MenuVisibility { get; set; }
        public ArticleOptions DisplayOptions { get; set; }
    }
}

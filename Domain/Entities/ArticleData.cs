using Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [NotMapped]
    public class ArticleData
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
        [Display(Name = "Visible")]
        public bool Visibility { get; set; }
        [Display(Name = "Add to menu")]
        public bool MenuVisibility { get; set; }
        public ArticleOptions DisplayOptions { get; set; }
    }
}

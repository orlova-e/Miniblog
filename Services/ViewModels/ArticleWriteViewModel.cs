using Domain.Entities;
using Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Miniblog.ViewModels
{
    public class ArticleWriteViewModel
    {
        [Required(ErrorMessage = "Header is required")]
        [StringLength(200, ErrorMessage = "Header's maximum length is 50 characters")]
        public string Header { get; set; }
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }
        public string Topic { get; set; }
        public string Series { get; set; }
        public string Tags { get; set; }
        public EntryType EntryType { get; set; }
        //public ColorTheme ColorTheme { get; set; }
        public bool Visibility { get; set; }
        public bool MenuVisibility { get; set; }
        public ArticleOptions DisplayOptions { get; set; }
    }
}

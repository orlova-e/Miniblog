using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Miniblog.ViewModels
{
    public class ArticleWriteViewModel
    {
        [Required, MaxLength(50)]
        public string Header { get; set; }
        [Required]
        public string Text { get; set; }
        public string Topic { get; set; }
        public EntryType EntryType { get; set; }
        public int ColorTheme { get; set; }
        public bool Visibility { get; set; }
        public bool MenuVisibility { get; set; }
        public string Link { get; set; }
        public string Tags { get; set; }
        public ArticleOptions DisplayOptions { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Miniblog.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public string ArticleId { get; set; }
        [Required]
        public string CommentId { get; set; }
        public string ParentId { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Avatar { get; set; }
        [Required]
        public string DateTime { get; set; }
        public string UpdatedDateTime { get; set; }
        [Required]
        public string Text { get; set; }
    }
}

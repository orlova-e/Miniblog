using System.ComponentModel.DataAnnotations;

namespace Miniblog.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public string ArticleId { get; set; }
        //[Required]
        public string CommentId { get; set; }
        public string Text { get; set; }
        public string ParentId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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

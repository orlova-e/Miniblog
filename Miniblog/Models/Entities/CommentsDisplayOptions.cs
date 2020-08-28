using Miniblog.Models.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class CommentsDisplayOptions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //public Guid WebsiteDisplayOptionsId { get; set; }
        //public WebsiteDisplayOptions WebsiteDisplayOptions { get; set; }
        public bool AllowNestedComments { get; set; }
        public int DepthOfNestedComments { get; set; }
        public SortingComments SortingCommentsDefaultType { get; set; }
    }
}

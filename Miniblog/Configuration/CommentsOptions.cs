using Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Configuration
{
    public class CommentsOptions
    {
        [Display(Name = "Allow nesting")]
        public bool AllowNesting { get; set; }
        public Changeable Depth { get; set; }
        public SortingComments SortingCommentsDefaultType { get; set; }
    }
}

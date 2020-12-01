using Miniblog.Models.Entities.Enums;

namespace Miniblog.Configuration
{
    public class CommentsOptions
    {
        public bool AllowNesting { get; set; }
        public Changeable Depth { get; set; }
        public SortingComments SortingCommentsDefaultType { get; set; }
    }
}

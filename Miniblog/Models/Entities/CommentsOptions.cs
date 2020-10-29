using Miniblog.Models.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class CommentsOptions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public bool AllowNesting { get; set; }
        public int Depth { get; set; }
        public SortingComments SortingCommentsDefaultType { get; set; }
    }
}

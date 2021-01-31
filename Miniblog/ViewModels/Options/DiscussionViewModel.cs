using System.Collections.Generic;
using Web.Configuration;

namespace Web.ViewModels.Options
{
    public class DiscussionViewModel
    {
        public CommentsOptions CommentsOptions { get; set; }
        public List<DiscussionRoles> DiscussionRoles { get; set; }
    }
}

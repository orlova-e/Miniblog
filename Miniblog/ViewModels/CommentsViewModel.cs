using Miniblog.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.ViewModels
{
    public class CommentsViewModel
    {
        public User CurrentUser { get; set; }
        public int Depth { get; set; }
        public bool WriteComments { get; set; }
        public List<Comment> Comments { get; set; }
    }
}

﻿using Miniblog.Models.Entities;
using System.Collections.Generic;

namespace Miniblog.ViewModels
{
    public class CommentsViewModel
    {
        public User User { get; set; }
        public int Depth { get; set; }
        public bool WriteComments { get; set; }
        public bool CommentsVisibility { get; set; }
        public List<Comment> Comments { get; set; }
    }
}

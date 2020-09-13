﻿using System;

namespace Miniblog.Models.Entities
{
    public class UserFavourite
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
    }
}
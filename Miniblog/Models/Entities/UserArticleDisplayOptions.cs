﻿using System;
using Miniblog.Models.Entities.Enums;

namespace Miniblog.Models.Entities
{
    public class UserArticleDisplayOptions : ArticlesDisplayOptions
    {
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }
        public ColorTheme ColorTheme { get; set; }
    }
}

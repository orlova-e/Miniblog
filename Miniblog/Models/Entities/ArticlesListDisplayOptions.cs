﻿using Miniblog.Models.Entities.Enums;

namespace Miniblog.Models.Entities
{
    public class ArticlesListDisplayOptions : ArticlesDisplayOptions
    {
        public int WebsiteDisplayOptionsId { get; set; }
        public WebsiteDisplayOptions WebsiteDisplayOptions { get; set; }
        public bool OverrideForUserArticle { get; set; }
        public byte ArticlesPerPage { get; set; }
        public byte WordsPerPreview { get; set; }
        public DisplayType ListDisplayDefaultType { get; set; }
        public ListLayoutType LayoutDefaultType { get; set; }
        public ListSortingType ArticlesListSortingDefaultType { get; set; }
    }
}

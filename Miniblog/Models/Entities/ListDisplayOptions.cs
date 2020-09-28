using Miniblog.Models.Entities.Enums;

namespace Miniblog.Models.Entities
{
    public class ListDisplayOptions : BaseArticlesOptions
    {
        public bool OverrideForUserArticle { get; set; }
        public byte ArticlesPerPage { get; set; }
        public byte WordsPerPreview { get; set; }
        public DisplayType ListDisplayDefaultType { get; set; }
        public ListLayoutType LayoutDefaultType { get; set; }
        public ListSortingType ListSortingDefaultType { get; set; }
    }
}

using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;

namespace Miniblog.Configuration
{
    public class ListOptions : BaseArticlesOptions
    {
        public bool OverrideForUserArticle { get; set; }
        public Changeable ArticlesPerPage { get; set; }
        public Changeable WordsPerPreview { get; set; }
        public DisplayType ListDisplayDefaultType { get; set; }
        public ListLayoutType LayoutDefaultType { get; set; }
        public ListSorting ListSortingDefaultType { get; set; }
    }
}

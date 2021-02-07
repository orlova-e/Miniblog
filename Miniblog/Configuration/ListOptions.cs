using Domain.Entities;
using Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Configuration
{
    public class ListOptions : BaseDisplayOptions
    {
        [Display(Name = "User can override some elements for their articles")]
        public bool OverrideForUserArticle { get; set; }
        public Changeable ArticlesPerPage { get; set; }
        public Changeable WordsPerPreview { get; set; }
        public DisplayType ListDisplayDefaultType { get; set; }
        public ListLayoutType LayoutDefaultType { get; set; }
        public ListSorting ListSortingDefaultType { get; set; }

        public static explicit operator ArticleOptions(ListOptions listOptions)
        {
            ArticleOptions articleOptions = new ArticleOptions()
            {
                Username = listOptions.Username,
                DateAndTime = listOptions.DateAndTime,
                Tags = listOptions.Tags,
                Topic = listOptions.Topic,
                Series = listOptions.Series,
                Likes = listOptions.Likes,
                Bookmarks = listOptions.Bookmarks,
                Comments = listOptions.Comments
            };
            return articleOptions;
        }
    }
}

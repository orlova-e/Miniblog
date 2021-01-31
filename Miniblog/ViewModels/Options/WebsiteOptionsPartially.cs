using Domain.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using Web.Configuration;

namespace Web.ViewModels.Options
{
    public class WebsiteOptionsPartially
    {
        [Display(Name = "Show list of popular and recent")]
        public bool ShowListOfPopularAndRecent { get; set; }
        [Display(Name = "Show authors")]
        public bool ShowAuthors { get; set; }
        [Display(Name = "Show topics")]
        public bool ShowTopics { get; set; }
        [Display(Name = "Show search option")]
        public bool ShowSearchOption { get; set; }
        [Range(0, 2)]
        public Visibility WebsiteVisibility { get; set; }

        public static explicit operator WebsiteOptionsPartially(WebsiteOptions websiteOptions)
            => new WebsiteOptionsPartially
            {
                ShowListOfPopularAndRecent = websiteOptions.ShowListOfPopularAndRecent,
                ShowAuthors = websiteOptions.ShowAuthors,
                ShowTopics = websiteOptions.ShowTopics,
                ShowSearchOption = websiteOptions.ShowSearchOption,
                WebsiteVisibility = websiteOptions.WebsiteVisibility
            };

        public static WebsiteOptions operator +(WebsiteOptions websiteOptions, WebsiteOptionsPartially partially)
        {
            websiteOptions.ShowListOfPopularAndRecent = partially.ShowListOfPopularAndRecent;
            websiteOptions.ShowAuthors = partially.ShowAuthors;
            websiteOptions.ShowTopics = partially.ShowTopics;
            websiteOptions.ShowSearchOption = partially.ShowSearchOption;
            websiteOptions.WebsiteVisibility = partially.WebsiteVisibility;

            return websiteOptions;
        }
    }
}

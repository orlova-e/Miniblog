using Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Web.Configuration
{
    public class WebsiteOptions
    {
        [Required]
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string IconPath { get; set; }
        [Required]
        public string StandardAvatarPath { get; set; }
        public string HomePage { get; set; }
        public bool ShowListOfPopularAndRecent { get; set; }
        public bool ShowAuthors { get; set; }
        public bool ShowTopics { get; set; }
        public bool ShowSearchOption { get; set; }
        public ColorTheme ColorTheme { get; set; }
        public Languages WebsiteLanguage { get; set; }
        public Visibility WebsiteVisibility { get; set; }
        [Required]
        public string WebsiteDateFormat { get; set; }
    }
}

using Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using Web.Configuration;
using Web.Infrastructure.Validation;

namespace Web.ViewModels.Options
{
    public class MainViewModel
    {
        [Required(ErrorMessage = "The title must be specified")]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        [Required(ErrorMessage = "The field must be specified")]
        [Range(0, 2, ErrorMessage = "Invalid range")]
        public Languages Language { get; set; }
        public string IconPath { get; set; }
        [MaxFileSize(1 * 1024 * 1024), AllowedFileExtensions("ico", "jpeg", "jpg", "png")]
        public IFormFile IconFile { get; set; }
        public string AvatarPath { get; set; }
        [MaxFileSize(1 * 1024 * 1024), AllowedFileExtensions("ico", "jpeg", "jpg", "png")]
        public IFormFile AvatarFile { get; set; }

        public static WebsiteOptions operator +(WebsiteOptions websiteOptions, MainViewModel mainViewModel)
        {
            websiteOptions.Name = mainViewModel.Title;
            websiteOptions.Subtitle = mainViewModel.Subtitle;
            websiteOptions.WebsiteLanguage = mainViewModel.Language;
            websiteOptions.IconPath = mainViewModel.IconPath;
            websiteOptions.StandardAvatarPath = mainViewModel.AvatarPath;

            return websiteOptions;
        }

        public static explicit operator MainViewModel(WebsiteOptions websiteOptions)
            => new MainViewModel()
            {
                Title = websiteOptions.Name,
                Subtitle = websiteOptions.Subtitle,
                Language = websiteOptions.WebsiteLanguage,
                IconPath = websiteOptions.IconPath,
                AvatarPath = websiteOptions.StandardAvatarPath
            };
    }
}

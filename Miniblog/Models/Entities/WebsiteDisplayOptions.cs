﻿using Miniblog.Models.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Miniblog.Models.Entities
{
    public class WebsiteDisplayOptions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public byte[] Icon { get; set; }
        public ArticlesListDisplayOptions ArticlesListDisplayOptions { get; set; }
        public string HomePage { get; set; }
        public bool HideListOfPopularAndRecent { get; set; }
        public bool HideSearchOption { get; set; }
        public ColorTheme ColorTheme { get; set; }
        public Languages WebsiteLanguage { get; set; }
        public Visibility WebsiteVisibility { get; set; }
        [Required]
        public string WebsiteDateFormat { get; set; }

        //public struct DateFormat
        //{
        //    public const string enUS = "mm/dd/yyyy";
        //    public const string ruRU = "dd.mm.yyyy";
        //    public const string frFR = "dd/mm/yyyy";
        //    public const string withGaps = "yyyy-mm-dd";
        //}
    }

}

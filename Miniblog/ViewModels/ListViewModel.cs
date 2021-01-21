﻿using Domain.Entities;
using Domain.Entities.Enums;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class ListViewModel
    {
        public string PageName { get; set; }
        public uint CurrentPageNumber { get; set; }
        public int TotalPages { get; set; }
        public ListSorting ListSortingType { get; set; }
        public List<Article> Articles { get; set; }
    }
}

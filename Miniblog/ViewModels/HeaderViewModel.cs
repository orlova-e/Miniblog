using Domain.Entities;
using System.Collections.Generic;

namespace Web.ViewModels
{
    public class HeaderViewModel
    {
        public string Title { get; set; }
        public User User { get; set; }
        public Dictionary<string, string> Pages { get; set; }
        public bool ShowSearch { get; set; }
    }
}

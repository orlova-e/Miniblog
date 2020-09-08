using System.Collections.Generic;

namespace Miniblog.ViewModels
{
    public class HeaderViewModel
    {
        public string Title { get; set; }
        public string Username { get; set; }
        public byte[] Avatar { get; set; }
        //public string Link { get; set; }
        public Dictionary<string, string> Pages { get; set; }
        public bool ShowSearch { get; set; }
    }
}

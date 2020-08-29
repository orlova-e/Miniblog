using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Entities
{
    public class ExtendedRole : Role
    {
        public bool ModerateArticles { get; set; }
        public bool ModerateComments { get; set; }
        public bool ModerateTopics { get; set; }
        public bool ModerateTags { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Miniblog.Requirements
{
    public class WriteArticleRequirement : IAuthorizationRequirement
    {
        protected internal bool WriteArticle { get; set; }
        public WriteArticleRequirement(bool writeArticle)
        {
            WriteArticle = writeArticle;
        }
    }
}

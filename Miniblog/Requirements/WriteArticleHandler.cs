using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Miniblog.Requirements
{
    public class WriteArticleHandler : AuthorizationHandler<WriteArticleRequirement>
    {
        public MiniblogDb _miniblogDb { get; set; }
        public WriteArticleHandler(MiniblogDb miniblogDb)
        {
            _miniblogDb = miniblogDb;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, WriteArticleRequirement requirement)
        {
            throw new NotImplementedException();
        }
    }
}

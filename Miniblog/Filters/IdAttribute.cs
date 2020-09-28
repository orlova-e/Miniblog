using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Miniblog.Models.Entities;
using Miniblog.Models.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Miniblog.Filters
{
    public class IdAttribute : IAsyncAuthorizationFilter
    {
        public IRepository repository { get; private set; }
        public IdAttribute(IRepository repository)
        {
            this.repository = repository;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                try
                {
                    Guid.TryParse(context.HttpContext.User.FindFirstValue("Id"), out Guid id);
                    User user = await repository.Users.GetByIdAsync(id);
                }
                catch (ArgumentNullException)
                {
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    //context.Result = new UnauthorizedResult();
                    context.Result = new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        }
    }
}

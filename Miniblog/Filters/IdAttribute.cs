using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Domain.Entities;
using Repo.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Services.Interfaces;

namespace Miniblog.Filters
{
    public class IdAttribute : IAsyncAuthorizationFilter
    {
        public IUserService userService { get; private set; }
        public IdAttribute(IUserService userService)
        {
            this.userService = userService;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                try
                {
                    Guid.TryParse(context.HttpContext.User.FindFirstValue("Id"), out Guid id);
                    User user = await userService.GetFromDbAsync(id);
                }
                catch (ArgumentNullException)
                {
                    await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    context.Result = new ChallengeResult(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            }
        }
    }
}

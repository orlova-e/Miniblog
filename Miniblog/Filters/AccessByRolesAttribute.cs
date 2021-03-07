using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Interfaces;
using System;
using System.Linq;

namespace Web.Filters
{
    public class AccessByRolesAttribute : IAuthorizationFilter
    {
        private IUserService UserService { get; }
        private readonly RoleType[] roles;
        public AccessByRolesAttribute(IUserService userService, RoleType[] types = null)
        {
            this.UserService = userService;
            roles = types;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (roles is not null && context.HttpContext.User.Identity.IsAuthenticated)
            {
                Role role = UserService.FindByName(context.HttpContext.User.Identity.Name).Role;
                if (!roles.Contains(role.Type))
                {
                    context.Result = new ForbidResult();
                }
            }
            else if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                Role role = UserService.FindByName(context.HttpContext.User.Identity.Name).Role;
                if (role is not ExtendedRole)
                {
                    context.Result = new ForbidResult();
                }
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }
}

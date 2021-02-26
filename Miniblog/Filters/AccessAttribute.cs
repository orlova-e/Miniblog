using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Web.Filters
{
    public class AccessAttribute : IAuthorizationFilter
    {
        private IUserService UserService { get; }
        private IEnumerable<string> Requirements { get; }
        public AccessAttribute(IUserService userService, string requirements)
        {
            UserService = userService;
            Requirements = requirements.Split(",").Select(r => r.Trim());

            IEnumerable<string> propertyNames = typeof(ExtendedRole)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(bool))
                .Select(p => p.Name);

            Requirements = Requirements.Intersect(propertyNames, StringComparer.OrdinalIgnoreCase);
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                Role role = UserService.FindByName(context.HttpContext.User.Identity.Name).Role;
                PropertyInfo[] propertyInfos = role.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                List<string> allowedActions = new List<string>();
                foreach (var property in propertyInfos)
                {
                    if (property.GetValue(role) is bool)
                    {
                        allowedActions.Add(property.Name);
                    }
                }

                var compared = Requirements.Except(allowedActions);

                if (compared.Any())
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}

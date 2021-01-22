using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace Web.Filters
{
    public class AccessAttribute : IAuthorizationFilter
    {
        public IOptionRepository<Role> RolesRepository { get; private set; }
        private List<string> Requirements { get; }
        public AccessAttribute(IOptionRepository<Role> rolesRepository, string requirements)
        {
            RolesRepository = rolesRepository;
            //_requirements = requirements.ToList();
            Requirements = requirements.Split(",").ToList();
            for (int i = 0; i < Requirements.Count(); i++)
                Requirements[i] = Requirements[i].Trim();
            PropertyInfo[] propertyInfos = (from property in typeof(ExtendedRole).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                            where property.PropertyType == typeof(bool)
                                            select property).ToArray();
            string[] propertyNames = new string[propertyInfos.Length];
            for (int i = 0; i < propertyNames.Length; i++)
                propertyNames[i] = propertyInfos[i].Name;
            Requirements = Requirements.Intersect(propertyNames, StringComparer.OrdinalIgnoreCase).ToList();
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string roleType = context.HttpContext.User.FindFirstValue(ClaimsIdentity.DefaultRoleClaimType);
                RoleType type = (RoleType)Enum.Parse(typeof(RoleType), roleType);
                Role role = RolesRepository.Find(r => r.Type == type).FirstOrDefault();
                PropertyInfo[] propertyInfos = (from property in role.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                where property.PropertyType == typeof(bool)
                                                select property).ToArray();
                List<string> allowedActions = new List<string>();
                foreach (var property in propertyInfos)
                    if ((bool)property.GetValue(role))
                        allowedActions.Add(property.Name);
                var compared = Requirements.Except(allowedActions);
                if (compared.Any())
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}

using Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Configuration;

namespace Web.App.Interfaces
{
    public interface ICommon
    {
        public BlogOptions Options { get; }
        public List<Role> Roles { get; }
        public List<ExtendedRole> ExtendedRoles { get; }
        Role GetRole(ClaimsPrincipal user);
    }
}

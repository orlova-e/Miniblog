using Miniblog.Models.Entities;
using Miniblog.Models.Entities.Enums;

namespace Miniblog.Configuration
{
    public class Roles
    {
        public Role User { get; set; }
        public ExtendedRole Editor { get; set; }
        public ExtendedRole Administrator { get; set; }
        public Role GetRole(RoleType roleType) =>
            roleType switch
            {
                RoleType.Editor => this.Editor,
                RoleType.Administrator => this.Administrator,
                _ => this.User
            };
    }
}

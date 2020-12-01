using Miniblog.Models.Entities;

namespace Miniblog.Configuration
{
    public class Roles
    {
        public Role User { get; set; }
        public ExtendedRole Editor { get; set; }
        public ExtendedRole Administrator { get; set; }
    }
}

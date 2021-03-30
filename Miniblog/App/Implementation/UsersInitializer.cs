using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.Extensions.Configuration;
using Repo;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Web.Configuration;

namespace Web.App.Implementation
{
    public class UsersInitializer
    {
        private readonly UserData _user, _editor, _administrator;
        private MiniblogDb _db { get; }

        public UsersInitializer(IConfiguration configuration,
            MiniblogDb db)
        {
            _db = db;

            _user = configuration.GetSection("Users").GetSection("User").Get<UserData>();
            _editor = configuration.GetSection("Users").GetSection("Editor").Get<UserData>();
            _administrator = configuration.GetSection("Users").GetSection("Administrator").Get<UserData>();

            _editor.Role = configuration.GetSection("Users:Editor:Role").Get<ExtendedRole>();
            _administrator.Role = configuration.GetSection("Users:Administrator:Role").Get<ExtendedRole>();
        }

        public void InitializeAndCheck()
        {
            static string GetHash(string password)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] fromPass = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return Encoding.UTF8.GetString(fromPass);
                }
            }

            _user.Password = GetHash(_user.Password);
            _editor.Password = GetHash(_editor.Password);
            _administrator.Password = GetHash(_administrator.Password);

            if (!_db.Roles.Where(r => r.Type == RoleType.User).Any())
            {
                _db.Roles.Add(_user.Role);
            }

            if (!_db.Roles.Where(r => r.Type == RoleType.Editor).Any())
            {
                _db.Roles.Add(_editor.Role);
            }

            if (!_db.Roles.Where(r => r.Type == RoleType.Administrator).Any())
            {
                _db.Roles.Add(_administrator.Role);
            }

            if (!_db.Users.Any())
            {
                _db.Users.AddRange(_user, _editor, _administrator);
            }
            else if (!_db.Users.Where(u => u.Role.Type == RoleType.Administrator).Any())
            {
                _db.Users.Add(_administrator);
            }

            _db.SaveChanges();
        }
    }
}

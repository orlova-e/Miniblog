using Domain.Entities;
using Domain.Entities.Enums;
using Repo.Interfaces;
using Services.Interfaces;
using Services.VisibleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementation
{
    public class UserService : IUserService
    {
        public IEntityObserver EntityObserver { get; }
        public IRepository repository { get; private set; }
        public UserService(IRepository repository,
            IEntityObserver entityObserver)
        {
            this.repository = repository;
            EntityObserver = entityObserver;
        }

        public Dictionary<string, string> CheckParameters(Account account)
        {
            var sameUsername = repository.Users.Find(u => u.Username == account.Username);
            var sameEmail = repository.Users.Find(u => u.Email == account.Email);
            Dictionary<string, string> errors = new Dictionary<string, string>();

            if (sameUsername.Any())
                errors.Add("Username", "A user with this name already exists");
            if (sameEmail.Any())
                errors.Add("Email", "A user with this email already exists");

            return errors;
        }

        public Dictionary<string, string> CheckParameters(User oldInfo, Account newInfo, string oldPassword = null)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            if (!oldInfo.Email.Equals(newInfo.Email))
            {
                var sameEmail = repository.Users.Find(u => u.Email == newInfo.Email);
                if (sameEmail.Any())
                    errors.Add(nameof(newInfo.Email), "A user with this email already exists");
            }

            if (!string.IsNullOrWhiteSpace(newInfo.Password) && !string.IsNullOrWhiteSpace(oldPassword))
            {
                string oldPasswordHash = GetHash(oldPassword);
                if (!oldInfo.Hash.Equals(oldPasswordHash))
                    errors.Add("OldPassword", "Passwords don't match");
            }

            return errors;
        }

        public async Task<User> CreateIntoDbAsync(Account account)
        {
            User newUser = ToUser(account);
            await repository.Users.CreateAsync(newUser);
            User user = repository.Users.Find(u => u.Email == newUser.Email && u.Hash == newUser.Hash).First();
            await EntityObserver.OnNewEntryAsync((VisibleUserValues)user);
            return user;
        }

        public async Task<User> UpdateAsync(User user, string newPassword = null)
        {
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.Hash = GetHash(newPassword);
            }

            await repository.Users.UpdateAsync(user);
            user = await repository.Users.GetByIdAsync(user.Id);
            await EntityObserver.OnUpdateAsync((VisibleUserValues)user);
            return user;
        }

        public async Task DeleteUserAsync(Guid id)
        {
            User user = await repository.Users.GetByIdAsync(id);
            if(user is not null)
            {
                await EntityObserver.OnDeleteAsync((VisibleUserValues)user);
                await repository.Users.DeleteAsync(id);
            }
        }

        public string GetHash(string password)
        {
            byte[] pass = Encoding.UTF8.GetBytes(password);
            string hash;
            using (SHA256 sha = SHA256.Create())
            {
                byte[] fromPass = sha.ComputeHash(pass);
                hash = Encoding.UTF8.GetString(fromPass);
            }
            return hash;
        }

        private User ToUser(Account account)
        {
            Role userRole = repository.Roles.Find(r => r.Type == RoleType.User).First();

            User newUser = new User()
            {
                Username = account.Username,
                Email = account.Email,
                RoleId = userRole.Id,
                Role = userRole,
                Hash = GetHash(account.Password),
                DateOfRegistration = DateTimeOffset.UtcNow
            };

            return newUser;
        }

        public User GetFromDb(Account account)
        {
            string hash = GetHash(account.Password);
            User user = repository.Users.Find(u => u.Username == account.Username && u.Hash == hash).FirstOrDefault();
            Role role;
            if (user != null)
            {
                role = repository.Roles.GetByIdAsync(user.RoleId).Result;
                user.Role = role;
            }
            return user;
        }

        public async Task<User> GetFromDbAsync(Guid id)
        {
            return await repository.Users.GetByIdAsync(id);
        }

        public User GetUserFromDb(Func<User, bool> predicate)
        {
            return repository.Users.Find(predicate).FirstOrDefault();
        }

        public User FindByName(string username)
        {
            User user = repository.Users.Find(u => u.Username.Equals(username)).FirstOrDefault();
            return user;
        }
    }
}

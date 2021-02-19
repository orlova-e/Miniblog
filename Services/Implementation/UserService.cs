﻿using Domain.Entities;
using Domain.Entities.Enums;
using Repo.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Services.Interfaces.Indexing;
using Services.VisibleValues;

namespace Services.Implementation
{
    public class UserService : IUserService
    {
        public IVisibleObjectsObserver IndexedObjectsObserver { get; }
        public IRepository repository { get; private set; }
        public UserService(IRepository repository,
            IVisibleObjectsObserver indexedObjectsObserver)
        {
            this.repository = repository;
            IndexedObjectsObserver = indexedObjectsObserver;
        }

        public Dictionary<string, string> CheckParameters(Account account)
        {
            var sameUsername = repository.Users.Find(u => u.Username == account.Username);
            var sameEmail = repository.Users.Find(u => u.Email == account.Email);
            Dictionary<string, string> errors = new Dictionary<string, string>();

            if(sameUsername.Any())
                errors.Add("Username", "A user with this name already exists");
            if(sameEmail.Any())
                errors.Add("Email", "A user with this email already exists");

            return errors;
        }

        public Dictionary<string, string> CheckParameters(User oldInfo, Account newInfo, string oldPassword = null)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            if (!oldInfo.Email.Equals(newInfo.Email))
            {
                var sameEmail = repository.Users.Find(u => u.Email == newInfo.Email);
                if(sameEmail.Any())
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
            User user = repository.Users.Find(u => u.Email == newUser.Email && u.Hash == newUser.Hash).First(); // user with Id generated by db
            await IndexedObjectsObserver.CheckNewEntityAsync((VisibleUserValues)user);
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
            await IndexedObjectsObserver.CheckUpdatedEntityAsync((VisibleUserValues)user);
            return user;
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
            Role userRole = repository.Roles.Find(r=> r.Type == RoleType.User).First();

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
            if(user != null)
            {
                role = repository.Roles.GetByIdAsync(user.RoleId).Result;
                user.Role = role;
            }
            return user;
        }

        //public bool CheckForExistence(Account account, Guid guid)
        //{
        //    string hash = GetHash(account.Password);
        //    var user = repository.Users.Find(u => u.Username == account.Username
        //        && u.Hash == hash
        //        && u.Id == guid)
        //        .FirstOrDefault();
        //    if(user != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public bool CheckForExistence(LoginViewModel loginModel, Guid guid)
        //{
        //    string hash = GetHash(loginModel.Password);
        //    var user = repository.Users.Find(u => u.Username == loginModel.Username
        //        && u.Hash == hash
        //        && u.Id == guid)
        //        .FirstOrDefault();
        //    if (user != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

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

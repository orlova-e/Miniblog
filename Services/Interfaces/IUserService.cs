using Domain.Entities;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Checks if users with same emails or usernames are already exists in database.
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns>List of errors.</returns>
        IEnumerable<string> ParametersAlreadyExist(RegisterViewModel registerModel);
        bool CheckForExistence(RegisterViewModel registerModel, Guid guid);
        bool CheckForExistence(LoginViewModel loginModel, Guid guid);
        /// <summary>
        /// Retrieves the hash from the password, gives plain user role and puts new user into database.
        /// </summary>
        /// <param name="registerModel"></param>
        /// <returns>New user object</returns>
        Task<User> CreateIntoDbAsync(RegisterViewModel registerModel);
        User GetFromDb(LoginViewModel loginModel);
        User GetUserFromDb(Func<User, bool> predicate);
        Task<User> GetFromDbAsync(Guid id);
        string GetHash(string password);
        Task AddSubscriberAsync(Guid authorId, Guid subscriberId);
        Task RemoveSubscriberAsync(Guid authorId, Guid subscriberId);
        Task<List<User>> GetSubscriptionAsync(Guid userId);


        //Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(Guid id);
        //Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(User user);
        //Task<User> GetUserInfoAsync(Guid id);
        //Task<User> GetUserInfoAsync(User user);
    }
}

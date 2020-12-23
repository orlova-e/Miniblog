using Domain.Entities;
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
        /// <param name="account"></param>
        /// <returns>List of errors.</returns>
        IEnumerable<string> CheckForInvalidAccountParameters(Account account);
        //bool CheckForExistence(Account account, Guid guid);
        //bool CheckForExistence(LoginViewModel loginModel, Guid guid);

        /// <summary>
        /// Retrieves the hash from the password, gives plain user role and puts new user into database.
        /// </summary>
        /// <param name="account"></param>
        /// <returns>New user object</returns>
        Task<User> CreateIntoDbAsync(Account account);
        User GetFromDb(Account account);
        User GetUserFromDb(Func<User, bool> predicate);
        Task<User> GetFromDbAsync(Guid id);
        User FindByName(string username);
        string GetHash(string password);
        Task AddSubscriberAsync(Guid authorId, Guid subscriberId);
        Task RemoveSubscriberAsync(Guid authorId, Guid subscriberId);
        Task<List<User>> GetSubscriptionAsync(Guid userId);
    }
}

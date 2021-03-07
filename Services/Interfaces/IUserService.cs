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
        /// <returns>Collection of errors if any, or empty collection.</returns>
        Dictionary<string, string> CheckParameters(Account account);
        /// <summary>
        /// Checks if users with same emails are already exists in database, checks if the old password matches
        /// </summary>
        /// <param name="oldInfo">Current user's entity</param>
        /// <param name="newInfo">Updated user's data that required verification</param>
        /// <param name="oldPassword"></param>
        /// <returns>Collection of errors if any, or empty collection.</returns>
        Dictionary<string, string> CheckParameters(User oldInfo, Account newInfo, string oldPassword = null);
        /// <summary>
        /// Retrieves the hash from the password, gives plain user role and puts new user into database.
        /// </summary>
        /// <param name="account"></param>
        /// <returns>New user object</returns>
        Task<User> CreateIntoDbAsync(Account account);
        Task<User> UpdateAsync(User user, string newPassword = null);
        Task DeleteUserAsync(Guid id);
        User GetFromDb(Account account);
        User GetUserFromDb(Func<User, bool> predicate);
        Task<User> GetFromDbAsync(Guid id);
        User FindByName(string username);
        string GetHash(string password);
    }
}

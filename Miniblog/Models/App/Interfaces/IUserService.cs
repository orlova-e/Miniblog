using Miniblog.Models.Entities;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miniblog.Models.App.Interfaces
{
    public interface IUserService
    {
        //Task<bool> AvailableInDb();
        //string GetHash();
        IEnumerable<string> ParametersAlreadyExist(RegisterModel registerModel);
        bool CheckForExistence(RegisterModel registerModel, Guid guid);
        bool CheckForExistence(LoginModel loginModel, Guid guid);
        Task<User> CreateIntoDbAsync(RegisterModel registerModel);
        User GetFromDb(LoginModel loginModel);
        Task<User> GetFromDbAsync(Guid id);
        Task PutRefreshTokenIntoDb(User user, string refreshToken, DateTimeOffset expiration);
        //Task ValidateRefreshTokenb(User user, string refreshToken);

        //Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(Guid id);
        //Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(User user);
        //Task<User> GetUserInfoAsync(Guid id);
        //Task<User> GetUserInfoAsync(User user);
    }
}

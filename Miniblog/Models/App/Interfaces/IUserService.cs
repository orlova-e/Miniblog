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
        IEnumerable<string> ParametersAlreadyExist(RegisterViewModel registerModel);
        bool CheckForExistence(RegisterViewModel registerModel, Guid guid);
        bool CheckForExistence(LoginViewModel loginModel, Guid guid);
        Task<User> CreateIntoDbAsync(RegisterViewModel registerModel);
        User GetFromDb(LoginViewModel loginModel);
        Task<User> GetFromDbAsync(Guid id);

        //Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(Guid id);
        //Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(User user);
        //Task<User> GetUserInfoAsync(Guid id);
        //Task<User> GetUserInfoAsync(User user);
    }
}

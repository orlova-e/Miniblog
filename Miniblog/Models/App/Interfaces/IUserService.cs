using Miniblog.Models.Entities;
using Miniblog.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miniblog.Models.App.Interfaces
{
    interface IUserService
    {
        //Task<bool> AvailableInDb();
        //string GetHash();
        Task PutToDbAsync(RegisterViewModel registerViewModel);
        Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(Guid id);
        Task<(User, IEnumerable<Article>)> GetAllUserInfoAsync(User user);
        Task<User> GetUserInfoAsync(Guid id);
        Task<User> GetUserInfoAsync(User user);
    }
}

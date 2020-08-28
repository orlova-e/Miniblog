﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    public interface IPlainRepository<T> where T: class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Find(Func<T, bool> predicate);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}

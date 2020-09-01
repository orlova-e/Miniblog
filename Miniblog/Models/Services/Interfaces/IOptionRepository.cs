using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    public interface IOptionRepository<T> where T: class
    {
        Task<T> GetByIdAsync(Guid id);
        IEnumerable<T> Find(Func<T, bool> predicate);
        //T FindFirst(Func<T, bool> predicate);
        Task UpdateAsync(T entity);
    }
}

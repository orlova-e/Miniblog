using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repo.Interfaces
{
    public interface IOptionRepository<T> where T: class
    {
        Task<T> FirstOrDefaultAsync();
        Task<T> GetByIdAsync(Guid id);
        IEnumerable<T> Find(Func<T, bool> predicate);
        Task UpdateAsync(T entity);
        Task<List<T>> GetAllAsync();
    }
}

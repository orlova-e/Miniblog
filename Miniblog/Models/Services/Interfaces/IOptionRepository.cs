using System;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    public interface IOptionRepository<T> where T: class
    {
        Task<T> GetByIdAsync(Guid id);
        Task UpdateAsync(T entity);
    }
}

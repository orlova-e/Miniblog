using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repo.Interfaces
{
    public interface IWorkingWithRange<T> : IPlainRepository<T>
        where T : class
    {
        Task CreateRangeAsync(IEnumerable<T> entities);
        Task UpdateRangeAsync(IEnumerable<T> entities);
        Task DeleteRangeAsync(IEnumerable<T> entities);
    }
}

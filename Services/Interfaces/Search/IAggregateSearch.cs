using Services.FoundValues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces.Search
{
    public interface IAggregateSearch<T>
        where T : class, new()
    {
        Task<List<FoundObject<T>>> FindAsync(string query, bool accurateSearch = false);
    }
}

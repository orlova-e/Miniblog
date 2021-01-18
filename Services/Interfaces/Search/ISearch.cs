using System.Collections.Generic;
using Services.FoundValues;
using System.Threading.Tasks;

namespace Services.Interfaces.Search
{
    public interface ISearch<T>
        where T : class, new()
    {
        public Task<List<FoundObject<T>>> FindAsync(string query);
    }
}

using Domain.Entities;
using Services.FoundValues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces.Search
{
    public interface IAggregateSearch<T>
        where T : Entity, new()
    {
        Task<List<FoundObject>> FindAsync(string query, bool accurateSearch = false);
    }
}

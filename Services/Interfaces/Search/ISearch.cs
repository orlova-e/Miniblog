using Domain.Entities;
using Services.FoundValues;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces.Search
{
    public interface ISearch<T>
        where T : Entity
    {
        public Task<List<FoundObject>> FindAsync(string query);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces.Search
{
    public interface ISearchStrategy<TEntity>
        where TEntity : class, new()
    {
        public List<Func<TEntity, bool>> Predicates { get; }
        public Func<Func<TEntity, bool>, Task<IEnumerable<TEntity>>> FindAsync { get; }
    }
}

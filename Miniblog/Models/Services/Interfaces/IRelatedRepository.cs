using Miniblog.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    public interface IRelatedRepository<TRelated, TEntry> where TRelated: class 
                                                        where TEntry: class
    {
        //Task<TRelated> GetByIdAsync(Guid id);
        //Task<IEnumerable<TRelated>> GetAllAsync();
        //IEnumerable<TRelated> Find(Func<TRelated, bool> predicate);
        //IEnumerable<TRelated> Find(TEntry entry, User user);
        //Task<int> UserCountAsync(TEntry entry);
        Task<List<TRelated>> GetAsync(Guid entryId);
        Task<IEnumerable<TEntry>> GetAllForAsync(Guid userId);
        Task<IEnumerable<User>> GetAllUserForAsync(Guid entryId);
        int Count(Guid entryId);
        //Task<int> UserCountAsync(Guid id);
        //Task<int> UserCountAsync(TRelated related/*, TEntry entry*/);
        //IEnumerable<TRelated> Find(Func<TRelated, E, bool> predicate);
        //Task<Guid> CreateAsync(User user, TEntry entry);
        //Task UpdateAsync(TRelated related);
        //Task UpdateAsync(User user, TEntry entry);
        Task AddFor(Guid entryId, Guid userId);
        Task RemoveFor(Guid entryId, Guid userId);
        //Task DeleteAsync(Guid id);
    }
}

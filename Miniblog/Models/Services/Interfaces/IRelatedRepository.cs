using Miniblog.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Miniblog.Models.Services.Interfaces
{
    public interface IRelatedRepository<TRelated, TEntry> where TRelated: class 
                                                        where TEntry: class
    {
        Task<List<TRelated>> GetAsync(Guid entryId);
        Task<IEnumerable<TEntry>> GetAllEntriesForAsync(Guid userId);
        Task<IEnumerable<User>> GetAllUsersForAsync(Guid entryId);
        /// <summary>
        /// Counts number of related entries
        /// </summary>
        /// <param name="entryId">Entry for which we count the number</param>
        /// <returns>Number of related entries</returns>
        int Count(Guid entryId);
        Task AddForAsync(Guid entryId, Guid userId);
        Task RemoveForAsync(Guid entryId, Guid userId);
        /// <summary>
        /// Checks whether the user has established a relationship
        /// </summary>
        /// <param name="entryId">Entry's id</param>
        /// <param name="userId">User's id</param>
        /// <returns>True if the label exists, false if it doesn't exist</returns>
        Task<bool> ContainsAsync(Guid entryId, Guid userId);
    }
}

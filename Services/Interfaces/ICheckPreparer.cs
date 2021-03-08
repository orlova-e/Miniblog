using Domain.Entities;
using Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICheckPreparer
    {
        Task AcceptAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task ChangeRole(Guid userId, RoleType type);
        IEnumerable<Entity> EnumerationOf(Func<Entity, bool> predicate);
        Task<IEnumerable<Entity>> SearchAsync(string query);
        Entity GetNext(Guid id, Guid[] existElementsIds);
        bool HasAccess(string queueList = null);
    }
}

using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repo.Interfaces
{
    public interface ISubscriptionsRepository
    {
        Task AddSubscriberAsync(Guid authorId, Guid subscriberId);
        Task RemoveSubscriberAsync(Guid authorId, Guid subscriberId);
        Task<List<User>> GetSubscriptionAsync(Guid userId);
    }
}

using Domain.Entities;
using Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repo.Implementation
{
    public class SubscriptionsRepository : ISubscriptionsRepository
    {
        public IRepository repository { get; private set; }
        public SubscriptionsRepository(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task AddSubscriberAsync(Guid authorId, Guid subscriberId)
        {
            User author = await repository.Users.GetByIdAsync(authorId);
            User subscriber = await repository.Users.GetByIdAsync(subscriberId);
            if (!author.Subscribers.Contains(subscriber))
                author.Subscribers.Add(subscriber);
            await repository.Users.UpdateAsync(author);
        }

        public async Task RemoveSubscriberAsync(Guid authorId, Guid subscriberId)
        {
            User author = await repository.Users.GetByIdAsync(authorId);
            User subscriber = await repository.Users.GetByIdAsync(subscriberId);
            if (author.Subscribers.Contains(subscriber))
                author.Subscribers.Remove(subscriber);
            await repository.Users.UpdateAsync(author);
        }

        public async Task<List<User>> GetSubscriptionAsync(Guid userId)
        {
            User user = await repository.Users.GetByIdAsync(userId);
            List<User> subscription = repository.Users.Find(u => u.Subscribers.Contains(user)).ToList();
            return subscription;
        }
    }
}

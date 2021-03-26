using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Repo.Interfaces;
using Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Web.Hubs
{
    [AllowAnonymous]
    public class SubscriptionHub : Hub
    {
        public IUserService UserService { get; private set; }
        public IRepository Repository { get; set; }

        public SubscriptionHub(IUserService userService,
            IRepository repository)
        {
            UserService = userService;
            Repository = repository;
        }

        [Authorize]
        public async Task Subscribe(string authorName)
        {
            bool wasSubscribed = false;

            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid subscriberId);

            User subscriber = UserService.GetUserFromDb(u => u.Id == subscriberId);
            User author = UserService.GetUserFromDb(u => u.Username.Equals(authorName));

            if (subscriber != null && author != null && !subscriber.Username.Equals(author.Username))
            {
                await Repository.Subscriptions.AddSubscriberAsync(author.Id, subscriberId);
                wasSubscribed = true;
            }

            await Clients.Caller.SendAsync("Subscribed", wasSubscribed);
        }

        [Authorize]
        public async Task Unsubscribe(string authorName)
        {
            bool wasUnsubscribed = false;

            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid subscriberId);

            User subscriber = UserService.GetUserFromDb(u => u.Id == subscriberId);
            User author = UserService.GetUserFromDb(u => u.Username.Equals(authorName));

            if (subscriber is not null && author is not null && !subscriber.Username.Equals(author.Username))
            {
                await Repository.Subscriptions.RemoveSubscriberAsync(author.Id, subscriberId);
                wasUnsubscribed = true;
            }

            await Clients.Caller.SendAsync("Unsubscribed", wasUnsubscribed);
        }

        [AllowAnonymous]
        public async Task Count(string authorName)
        {
            User author = UserService.GetUserFromDb(u => u.Username.Equals(authorName));
            int number = author?.Subscribers.Count ?? 0;
            await Clients.All.SendAsync("Counted", number);
        }
    }
}

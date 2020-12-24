using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Services.Interfaces;
using Domain.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Repo.Interfaces;

namespace Miniblog.Hubs
{
    [AllowAnonymous]
    public class SubscriptionHub : Hub
    {
        public IUserService userService { get; private set; }
        public IRepository repository { get; set; }

        public SubscriptionHub(IUserService userService,
            IRepository repository)
        {
            this.userService = userService;
            this.repository = repository;
        }

        [Authorize]
        public async Task Subscribe(string authorName)
        {
            int statusCode = StatusCodes.Status404NotFound;

            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid subscriberId);

            User subscriber = userService.GetUserFromDb(u => u.Id == subscriberId);
            User author = userService.GetUserFromDb(u => u.Username.Equals(authorName));

            if(subscriber != null && author != null && !subscriber.Username.Equals(author.Username))
            {
                await repository.Subscriptions.AddSubscriberAsync(author.Id, subscriberId);
                statusCode = StatusCodes.Status200OK;
            }

            //await Clients.Caller.SendAsync("Subscribed", statusCode);
            await Clients.User(Context.UserIdentifier).SendAsync("Subscribed", statusCode);
        }

        [Authorize]
        public async Task Unsubscribe(string authorName)
        {
            int statusCode = StatusCodes.Status404NotFound;

            Guid.TryParse(Context.User.FindFirstValue("Id"), out Guid subscriberId);

            User subscriber = userService.GetUserFromDb(u => u.Id == subscriberId);
            User author = userService.GetUserFromDb(u => u.Username.Equals(authorName));

            if (subscriber != null && author != null && !subscriber.Username.Equals(author.Username))
            {
                await repository.Subscriptions.RemoveSubscriberAsync(author.Id, subscriberId);
                statusCode = StatusCodes.Status200OK;
            }

            //await Clients.Caller.SendAsync("Unsubscribed", statusCode);
            await Clients.User(Context.UserIdentifier).SendAsync("Unsubscribed", statusCode);
        }

        [AllowAnonymous]
        public async Task Count(string authorName)
        {
            User author = userService.GetUserFromDb(u => u.Username.Equals(authorName));
            int number = author?.Subscribers.Count ?? 0;
            await Clients.All.SendAsync("Counted", number);
        }
    }
}

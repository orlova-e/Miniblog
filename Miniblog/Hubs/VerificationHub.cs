using Domain.Entities;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Services.Implementation;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.ViewModels.Options;

namespace Web.Hubs
{
    [Authorize]
    public class VerificationHub : Hub
    {
        private string queueList;
        private ExtendedRole role;
        private CheckPreparerBuilder PreparerBuilder { get; }
        public IUserService UserService { get; }
        public VerificationHub(CheckPreparerBuilder builder,
            IUserService userService)
        {
            PreparerBuilder = builder;
            UserService = userService;
        }

        private ICheckPreparer GetPreparer()
        {
            queueList = Context.GetHttpContext().Request.Query[nameof(queueList)];
            User user = UserService.FindByName(Context.UserIdentifier);
            ICheckPreparer preparer = PreparerBuilder.Build(user, queueList);
            if (!preparer.HasAccess())
            {
                throw new Exception();
            }
            role = user.Role as ExtendedRole;

            return preparer;
        }

        public async Task Accept(Guid id, Guid[] existElementsIds)
        {
            ICheckPreparer queuePreparer = GetPreparer();

            await queuePreparer.AcceptAsync(id);
            Verifiable next = queuePreparer.GetNext(id, existElementsIds);

            await Clients.All.SendAsync("AcceptedOrDeleted", id, next);
        }

        public async Task Delete(Guid id, Guid[] existElementsIds)
        {
            ICheckPreparer queuePreparer = GetPreparer();

            await queuePreparer.DeleteAsync(id);
            Verifiable next = queuePreparer.GetNext(id, existElementsIds);

            await Clients.All.SendAsync("AcceptedOrDeleted", id, next);
        }

        [Authorize(Roles = nameof(RoleType.Administrator))]
        public async Task ChangeRole(Guid userId, RoleType type)
        {
            ICheckPreparer queuePreparer = GetPreparer();
            await queuePreparer.ChangeRole(userId, type);
        }

        public async Task Search(string query)
        {
            ICheckPreparer queuePreparer = GetPreparer();

            int number = 10;
            IEnumerable<Entity> entities = await queuePreparer.SearchAsync(query);
            entities = entities?.Take(number);

            if (role.Type is RoleType.Administrator && queueList is "users")
            {
                IEnumerable<VerifiableUser> verifiables = entities?.Select(e => (VerifiableUser)(User)e);
                await Clients.Caller.SendAsync("SearchResults", verifiables);
            }
            else
            {
                IEnumerable<Verifiable> verifiables = entities?.Select(e => (Verifiable)e);
                await Clients.Caller.SendAsync("SearchResults", verifiables);
            }
        }
    }
}

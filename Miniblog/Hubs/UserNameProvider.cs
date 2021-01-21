using Microsoft.AspNetCore.SignalR;

namespace Web.Hubs
{
    public class UserNameProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity.Name;
        }
    }
}

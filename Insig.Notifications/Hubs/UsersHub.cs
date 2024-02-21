using System;
using System.Threading.Tasks;
using Insig.Common.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Insig.Notifications.Hubs;

[Authorize]
public class UsersHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userSub = Context.User?.FindFirst(ClaimsType.Sub)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, userSub);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userSub = Context.User?.FindFirst(ClaimsType.Sub)?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userSub);
        await base.OnDisconnectedAsync(exception);
    }
}

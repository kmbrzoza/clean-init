using System.Collections.Generic;
using System.Threading.Tasks;
using Insig.Notifications.Hubs;
using Insig.Notifications.INotifiers;
using Insig.Notifications.Models;
using Microsoft.AspNetCore.SignalR;

namespace Insig.Notifications.Notifiers;

public class UsersHubNotifier : IUsersHubNotifier
{
    private readonly IHubContext<UsersHub> _hubContext;

    public UsersHubNotifier(IHubContext<UsersHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyUser(string userSub, string title, string body) =>
        await _hubContext.Clients.Groups(userSub)
            .SendAsync("UserNotification", new NotificationMessage(title, body));

    public async Task NotifyUsers(IEnumerable<string> userSubs, string title, string body) =>
        await _hubContext.Clients.Groups(userSubs)
            .SendAsync("UserNotification", new NotificationMessage(title, body));
}

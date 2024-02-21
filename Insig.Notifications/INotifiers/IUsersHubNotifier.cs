using System.Collections.Generic;
using System.Threading.Tasks;

namespace Insig.Notifications.INotifiers;

public interface IUsersHubNotifier
{
    Task NotifyUser(string userSub, string title, string body);
    Task NotifyUsers(IEnumerable<string> userSubs, string title, string body);
}

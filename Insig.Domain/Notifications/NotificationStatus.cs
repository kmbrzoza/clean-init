using Insig.Domain.Lookups;

namespace Insig.Domain.Notifications;

public class NotificationStatus : LookupType
{
    public NotificationStatus(int id, string name) : base(id, name) { }
}

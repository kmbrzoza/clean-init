using System.Threading.Tasks;
using Insig.Domain.Notifications;

namespace Insig.ApplicationServices.Boundaries.Notifications;

public interface INotificationRepository
{
    Task Store(Notification notification);
    Task<Notification> Get(long id, long recipientId);
}

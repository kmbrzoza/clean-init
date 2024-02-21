using System.Threading.Tasks;
using Insig.Common.Infrastructure.QueryBuilder;
using Insig.PublishedLanguage.Dtos.Notifications;

namespace Insig.ApplicationServices.Boundaries.Notifications;

public interface INotificationQuery
{
    Task<Page<NotificationDTO>> GetUserNotifications(SearchCriteria searchCriteria, long userId);
    Task<long> GetPendingNotificationsCount(long userId);
}

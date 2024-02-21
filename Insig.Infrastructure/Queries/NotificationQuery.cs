using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Notifications;
using Insig.Common.Infrastructure.QueryBuilder;
using Insig.Common.Lookups;
using Insig.Infrastructure.QueryBuilder;
using Insig.PublishedLanguage.Dtos.Notifications;

namespace Insig.Infrastructure.Queries;

public class NotificationQuery : INotificationQuery
{
    private readonly SqlQueryBuilder _sqlQueryBuilder;

    public NotificationQuery(SqlQueryBuilder sqlQueryBuilder)
    {
        _sqlQueryBuilder = sqlQueryBuilder;
    }

    public async Task<Page<NotificationDTO>> GetUserNotifications(SearchCriteria searchCriteria, long userId)
    {
        return await _sqlQueryBuilder
            .SelectAllProperties<NotificationDTO>()
            .From("dbo.Notification")
            .Where("RecipientId", userId)
            .OrderBy(searchCriteria.OrderBy)
            .BuildPagedQuery<NotificationDTO>(searchCriteria)
            .Execute();
    }

    public async Task<long> GetPendingNotificationsCount(long userId)
    {
        return await _sqlQueryBuilder
            .Select("COUNT(*)")
            .From("dbo.Notification")
            .Where("RecipientId", userId)
            .Where("StatusId", NotificationStatusEnum.Pending)
            .BuildQuery<long>()
            .ExecuteToFirstElement();
    }
}

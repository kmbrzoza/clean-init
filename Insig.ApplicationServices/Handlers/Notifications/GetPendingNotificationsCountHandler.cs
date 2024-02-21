using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Notifications;
using Insig.Common.Auth;
using Insig.PublishedLanguage.Queries.Notifications;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Notifications;

public class GetPendingNotificationsCountHandler : IRequestHandler<GetPendingNotificationsCountQuery, long>
{
    private readonly INotificationQuery _notificationQuery;
    private readonly ICurrentUserService _currentUserService;

    public GetPendingNotificationsCountHandler(
        INotificationQuery notificationQuery,
        ICurrentUserService currentUserService)
    {
        _notificationQuery = notificationQuery;
        _currentUserService = currentUserService;
    }

    public async Task<long> Handle(GetPendingNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        return await _notificationQuery.GetPendingNotificationsCount(await _currentUserService.GetId());
    }
}

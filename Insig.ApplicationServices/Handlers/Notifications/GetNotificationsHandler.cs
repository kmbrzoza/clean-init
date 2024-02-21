using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Notifications;
using Insig.Common.Auth;
using Insig.Common.Infrastructure.QueryBuilder;
using Insig.PublishedLanguage.Dtos.Notifications;
using Insig.PublishedLanguage.Queries.Notifications;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Notifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, Page<NotificationDTO>>
{
    private readonly INotificationQuery _notificationQuery;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationsHandler(INotificationQuery notificationQuery, ICurrentUserService currentUserService)
    {
        _notificationQuery = notificationQuery;
        _currentUserService = currentUserService;
    }

    public async Task<Page<NotificationDTO>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _notificationQuery.GetUserNotifications(request, await _currentUserService.GetId());
    }
}

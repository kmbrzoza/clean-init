using Insig.Common.Infrastructure.QueryBuilder;
using Insig.PublishedLanguage.Dtos.Notifications;
using MediatR;

namespace Insig.PublishedLanguage.Queries.Notifications;

public class GetNotificationsQuery : SearchCriteria, IRequest<Page<NotificationDTO>>
{
}

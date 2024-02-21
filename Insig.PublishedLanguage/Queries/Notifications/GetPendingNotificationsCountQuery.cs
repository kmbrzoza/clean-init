using MediatR;

namespace Insig.PublishedLanguage.Queries.Notifications;

public class GetPendingNotificationsCountQuery : IRequest<long>
{
}

using MediatR;

namespace Insig.PublishedLanguage.Commands.Notifications;

public class MarkNotificationAsCompletedCommand : IRequest
{
    public long Id { get; set; }
}

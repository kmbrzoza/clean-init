using EnsureThat;
using Insig.Common.Lookups;
using Insig.Domain.Common;

namespace Insig.Domain.Notifications;

public class Notification : AuditableEntity
{
    private Notification() { }

    public Notification(NotificationRecipient recipient, NotificationMessage message, NotificationAuthor author = null, NotificationAction action = null)
    {
        EnsureArg.IsNotNull(recipient, nameof(recipient));
        EnsureArg.IsNotNull(message, nameof(message));

        StatusId = (int)NotificationStatusEnum.Pending;
        Recipient = recipient;
        Message = message;
        Author = author;
        Action = action;
    }

    public void MarkAsCompleted()
    {
        StatusId = (int)NotificationStatusEnum.Completed;
    }

    public long Id { get; }
    public int StatusId { get; private set; }
    public NotificationRecipient Recipient { get; }
    public NotificationMessage Message { get; }
    public NotificationAuthor Author { get; }
    public NotificationAction Action { get; }
}

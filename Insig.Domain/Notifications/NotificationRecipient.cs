using EnsureThat;

namespace Insig.Domain.Notifications;

public class NotificationRecipient
{
    private NotificationRecipient() { }

    public NotificationRecipient(long recipientId)
    {
        EnsureArg.IsNotDefault(recipientId, nameof(recipientId));

        RecipientId = recipientId;
    }

    public long RecipientId { get; }
}

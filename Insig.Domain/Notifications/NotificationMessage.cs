using EnsureThat;

namespace Insig.Domain.Notifications;

public class NotificationMessage
{
    private NotificationMessage() { }

    public NotificationMessage(string title, string body)
    {
        EnsureArg.IsNotNullOrWhiteSpace(title, nameof(title));
        EnsureArg.IsNotNullOrWhiteSpace(body, nameof(body));

        Title = title;
        Body = body;
    }

    public string Title { get; }
    public string Body { get; }
}

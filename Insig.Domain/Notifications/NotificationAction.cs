namespace Insig.Domain.Notifications;

public class NotificationAction
{
    private NotificationAction() { }

    public NotificationAction(string redirectUrl)
    {
        RedirectUrl = redirectUrl;
    }

    public string RedirectUrl { get; }
}

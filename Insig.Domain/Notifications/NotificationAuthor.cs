namespace Insig.Domain.Notifications;

public class NotificationAuthor
{
    private NotificationAuthor() { }

    public NotificationAuthor(long? authorId)
    {
        AuthorId = authorId;
    }

    public long? AuthorId { get; set; }
}

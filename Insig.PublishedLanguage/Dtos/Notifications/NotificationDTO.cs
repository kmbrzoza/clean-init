using System;

namespace Insig.PublishedLanguage.Dtos.Notifications;

public class NotificationDTO
{
    public long Id { get; set; }
    public int StatusId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string RedirectUrl { get; set; }
    public DateTime CreatedOn { get; set; }
}

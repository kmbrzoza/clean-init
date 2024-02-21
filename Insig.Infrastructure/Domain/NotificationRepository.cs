using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Notifications;
using Insig.Domain.Notifications;
using Insig.Infrastructure.DataModel.Context;
using Microsoft.EntityFrameworkCore;

namespace Insig.Infrastructure.Domain;

public class NotificationRepository : INotificationRepository
{
    private readonly InsigContext _context;

    public NotificationRepository(InsigContext context)
    {
        _context = context;
    }

    public async Task Store(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task<Notification> Get(long id, long recipientId)
    {
        return await _context.Notifications.SingleAsync(n => n.Id == id && n.Recipient.RecipientId == recipientId);
    }
}

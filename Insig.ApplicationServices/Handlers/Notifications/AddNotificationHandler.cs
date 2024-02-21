using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Notifications;
using Insig.Common.Auth;
using Insig.Domain;
using Insig.Domain.Notifications;
using Insig.Notifications.INotifiers;
using Insig.PublishedLanguage.Commands.Notifications;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Notifications;

public class AddNotificationHandler : IRequestHandler<AddNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsersHubNotifier _usersHubNotifier;

    public AddNotificationHandler(
        IUnitOfWork unitOfWork,
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService,
        IUsersHubNotifier usersHubNotifier)
    {
        _unitOfWork = unitOfWork;
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
        _usersHubNotifier = usersHubNotifier;
    }

    public async Task Handle(AddNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = await _currentUserService.GetId();

        var notification = CreateNotification(currentUserId);

        await _notificationRepository.Store(notification);

        await _unitOfWork.Save();

        await _usersHubNotifier.NotifyUser(_currentUserService.Sub, notification.Message.Title, notification.Message.Body);
    }

    private Notification CreateNotification(long userId)
    {
        return new Notification(
            new NotificationRecipient(userId),
            new NotificationMessage("Example notification", "Message of example notification")
        );
    }
}

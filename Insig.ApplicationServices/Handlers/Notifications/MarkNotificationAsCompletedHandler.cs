using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Notifications;
using Insig.Common.Auth;
using Insig.Domain;
using Insig.PublishedLanguage.Commands.Notifications;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Notifications;

public class MarkNotificationAsCompletedHandler : IRequestHandler<MarkNotificationAsCompletedCommand>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsCompletedHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MarkNotificationAsCompletedCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.Get(request.Id, await _currentUserService.GetId());

        notification.MarkAsCompleted();

        await _unitOfWork.Save();
    }
}

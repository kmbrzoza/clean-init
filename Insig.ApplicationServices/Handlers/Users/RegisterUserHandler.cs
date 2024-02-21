using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Users;
using Insig.Common.Auth;
using Insig.Domain;
using Insig.Domain.Users;
using Insig.PublishedLanguage.Commands.Users;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Users;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsersRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserHandler(
        ICurrentUserService currentUserService,
        IUsersRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(
            _currentUserService.Sub,
            _currentUserService.Email
        );

        await _userRepository.Store(user);

        await _unitOfWork.Save();
    }
}

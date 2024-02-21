using System.Threading;
using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Users;
using Insig.Common.Auth;
using Insig.PublishedLanguage.Dtos.Users;
using Insig.PublishedLanguage.Queries.Users;
using MediatR;

namespace Insig.ApplicationServices.Handlers.Users;

public class GetUserRoleHandler : IRequestHandler<GetUserRoleQuery, UserRoleDTO>
{
    private readonly IUsersQuery _usersQuery;
    private readonly ICurrentUserService _currentUserService;

    public GetUserRoleHandler(
        IUsersQuery usersQuery,
        ICurrentUserService currentUserService)
    {
        _usersQuery = usersQuery;
        _currentUserService = currentUserService;
    }

    public async Task<UserRoleDTO> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
    {
        if (!await _usersQuery.CheckIfUserExists(_currentUserService.Sub))
        {
            return null;
        }

        return await _usersQuery.GetUserRole(_currentUserService.Sub);
    }
}

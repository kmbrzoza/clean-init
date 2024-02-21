using Insig.PublishedLanguage.Dtos.Users;
using MediatR;

namespace Insig.PublishedLanguage.Queries.Users;

public class GetUserRoleQuery : IRequest<UserRoleDTO>
{
}
using System.Threading.Tasks;
using Insig.Common.Auth.Lookups;
using Insig.PublishedLanguage.Dtos.Users;

namespace Insig.ApplicationServices.Boundaries.Users;

public interface IUsersQuery
{
    Task<bool> CheckIfUserExists(string sub);
    Task<UserRoleDTO> GetUserRole(string sub);
    Task<RoleEnum?> GetUserRoleEnum(string sub);
    Task<long> GetUserId(string sub);
}
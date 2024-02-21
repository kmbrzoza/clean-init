using System.Threading.Tasks;
using Insig.Common.Auth.Lookups;

namespace Insig.Common.Auth;

public interface ICurrentUserService
{
    string Sub { get; }
    string Email { get; }
    string Token { get; }

    bool IsLogged();
    Task<long> GetId();
    Task<RoleEnum?> GetRole();
}
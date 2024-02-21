using System.Threading.Tasks;
using Insig.Domain.Users;

namespace Insig.ApplicationServices.Boundaries.Users;

public interface IUsersRepository
{
    Task Store(User user);
}

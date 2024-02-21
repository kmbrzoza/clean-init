using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Users;
using Insig.Domain.Users;
using Insig.Infrastructure.DataModel.Context;

namespace Insig.Infrastructure.Domain;

public class UsersRepository : IUsersRepository
{
    private readonly InsigContext _context;

    public UsersRepository(InsigContext context)
    {
        _context = context;
    }

    public async Task Store(User user)
    {
        await _context.Users.AddAsync(user);
    }
}

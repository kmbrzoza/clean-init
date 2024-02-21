using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Users;
using Insig.Common.Auth.Lookups;
using Insig.Infrastructure.QueryBuilder;
using Insig.PublishedLanguage.Dtos.Users;

namespace Insig.Infrastructure.Queries;

public class UsersQuery : IUsersQuery
{
    private readonly SqlQueryBuilder _sqlQueryBuilder;

    public UsersQuery(SqlQueryBuilder sqlQueryBuilder)
    {
        _sqlQueryBuilder = sqlQueryBuilder;
    }

    public async Task<bool> CheckIfUserExists(string sub)
    {
        return (await _sqlQueryBuilder
            .Select("Id")
            .From("[dbo].[User]")
            .Where("Sub", sub)
            .BuildQuery<long?>()
            .ExecuteToFirstElement()).HasValue;
    }

    public async Task<UserRoleDTO> GetUserRole(string sub)
    {
        return await _sqlQueryBuilder
            .Select("RoleId")
            .From("[dbo].[User]")
            .Where("Sub", sub)
            .BuildQuery<UserRoleDTO>()
            .ExecuteSingle();
    }

    public async Task<RoleEnum?> GetUserRoleEnum(string sub)
    {
        return await _sqlQueryBuilder
            .Select("RoleId")
            .From("[dbo].[User]")
            .Where("Sub", sub)
            .BuildQuery<RoleEnum?>()
            .ExecuteToFirstElement();
    }

    public async Task<long> GetUserId(string sub)
    {
        return await _sqlQueryBuilder
            .Select("Id")
            .From("[dbo].[User]")
            .Where("Sub", sub)
            .BuildQuery<long>()
            .ExecuteSingle();
    }
}
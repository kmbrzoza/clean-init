using System;
using System.Threading.Tasks;
using Insig.ApplicationServices.Boundaries.Users;
using Insig.ApplicationServices.Services;
using Insig.Common.Auth;
using Insig.Common.Auth.Lookups;
using Microsoft.AspNetCore.Http;

namespace Insig.Api.Infrastructure;

public class CurrentUserService : ICurrentUserService
{
    private readonly TimeSpan _cacheDataTimeToLive = TimeSpan.FromMinutes(5);
    private readonly IUsersQuery _usersQuery;
    private readonly ICacheService _cacheService;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUsersQuery usersQuery, ICacheService cacheService)
    {
        Sub = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimsType.Sub)?.Value;
        Email = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimsType.EmailAddress)?.Value;
        Token = httpContextAccessor.HttpContext?.Request?.Headers["Authorization"];
        _usersQuery = usersQuery;
        _cacheService = cacheService;
    }

    public string Sub { get; }
    public string Email { get; }
    public string Token { get; }

    public bool IsLogged() => Sub != null;

    public async Task<long> GetId() => _cacheService.GetCachedData($"{Token}_id") as long? ?? await ObtainId();

    public async Task<RoleEnum?> GetRole() => _cacheService.GetCachedData($"{Token}_role") as RoleEnum? ?? await ObtainRole();

    private async Task<long> ObtainId()
    {
        var id = await _usersQuery.GetUserId(Sub);
        _cacheService.CacheData($"{Token}_id", id, _cacheDataTimeToLive);
        return id;
    }

    private async Task<RoleEnum?> ObtainRole()
    {
        var role = await _usersQuery.GetUserRoleEnum(Sub);
        _cacheService.CacheData($"{Token}_role", role, _cacheDataTimeToLive);
        return role;
    }
}
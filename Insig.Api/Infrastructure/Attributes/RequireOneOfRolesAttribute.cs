using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insig.Common.Auth;
using Insig.Common.Auth.Lookups;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Insig.Api.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequireOneOfRolesAttribute : Attribute, IAsyncActionFilter
{
    private IEnumerable<RoleEnum> _roles;

    public RequireOneOfRolesAttribute(params RoleEnum[] roles)
    {
        if (!roles.Any())
        {
            throw new ArgumentException("Roles are missing");
        }

        _roles = roles;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var currentUserService = context.HttpContext.RequestServices.GetService<ICurrentUserService>();

        var userRole = await currentUserService.GetRole();

        if (DoesUserHaveOneOfRoles(userRole, _roles))
        {
            await next();
        }
        else
        {
            context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        }
    }

    private bool DoesUserHaveOneOfRoles(RoleEnum? userRole, IEnumerable<RoleEnum> roles)
    {
        if (userRole is null)
        {
            return false;
        }

        return roles.Any(role => role == userRole);
    }
}

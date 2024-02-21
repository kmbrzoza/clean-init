using Insig.Api.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Insig.Api.Infrastructure;

public static class ApiBuilderExtensions
{
    public static IApplicationBuilder UseExceptionLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionLoggingMiddleware>();
    }
}

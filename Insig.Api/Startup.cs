using System;
using System.Threading.Tasks;
using Autofac;
using Insig.Api.Infrastructure;
using Insig.ApplicationServices.Handlers.Users;
using Insig.Common.Auth;
using Insig.Common.Configuration;
using Insig.Notifications.Hubs;
using Insig.PublishedLanguage.Commands.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace Insig.Api;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddSignalR();
        services.AddHttpContextAccessor();
        services.AddMemoryCache();
        services.AddApplicationInsightsTelemetry();

        RegisterMediatR(services);
        ConfigureAuth(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseCors(b => b.WithOrigins(Configuration["AppConfig:ClientUrl"]).AllowAnyHeader().AllowAnyMethod().AllowCredentials());

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFilesForLocalFileStorage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseApiSecurityHttpHeaders();
        app.UseBlockingContentSecurityPolicyHttpHeader();
        app.RemoveServerHeader();
        app.UseNoCacheHttpHeaders();
        app.UseStrictTransportSecurityHttpHeader(env);
        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionLogger();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();

            endpoints.MapHub<UsersHub>("/hubs/notifications");
        });
    }

    public virtual void ConfigureAuth(IServiceCollection services)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "ApiAzureAd");

        ConfigureHubAuth(services);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.ApiReader, policy => policy.RequireScope(Scopes.InsigApi));
        });

        services.AddMvcCore(options =>
        {
            options.Filters.Add(new AuthorizeFilter(Policies.ApiReader));
        });
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new DefaultModule(Configuration));
    }

    private void ConfigureHubAuth(IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            Func<MessageReceivedContext, Task> existingOnMessageReceivedHandler = options.Events.OnMessageReceived;
            options.Events.OnMessageReceived = async context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                {
                    context.Token = accessToken;
                }

                await existingOnMessageReceivedHandler(context);
            };
        });
    }

    private void RegisterMediatR(IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(RegisterUserCommand).Assembly, typeof(RegisterUserHandler).Assembly));
    }
}
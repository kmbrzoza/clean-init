using System;
using System.Data;
using System.Reflection;
using Autofac;
using Azure.Storage.Blobs;
using EnsureThat;
using Insig.Api.Controllers;
using Insig.Common.Auth;
using Insig.Common.Infrastructure.Email;
using Insig.Infrastructure.DataModel.Context;
using Insig.Infrastructure.Domain;
using Insig.Infrastructure.FileProcessing.Services;
using Insig.Infrastructure.Queries;
using Insig.Infrastructure.QueryBuilder;
using Insig.Infrastructure.Services;
using Insig.Notifications.INotifiers;
using Insig.Notifications.Notifiers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace Insig.Api.Infrastructure;

public class DefaultModule : Module
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DefaultModule(IConfiguration configuration)
    {
        EnsureArg.IsNotNull(configuration, nameof(configuration));
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("KOPRO");
        Ensure.String.IsNotNullOrWhiteSpace(_connectionString, nameof(_connectionString));
    }

    protected override void Load(ContainerBuilder builder)
    {
        RegisterContext(builder);
        RegisterDatabaseAccess(builder);
        RegisterServices(builder);
        RegisterControllers(builder);
        RegisterQueries(builder);
        RegisterRepositories(builder);
        RegisterStorageService(builder);
        RegisterNotifications(builder);
        RegisterEmailServices(builder);
    }

    private static void RegisterTransientDependenciesAutomatically(
        ContainerBuilder builder,
        Assembly assembly,
        string nameSpace)
    {
        builder.RegisterAssemblyTypes(assembly)
            .Where(t => !string.IsNullOrEmpty(t.Namespace) && t.Namespace.StartsWith(nameSpace, StringComparison.InvariantCulture))
            .AsSelf()
            .AsImplementedInterfaces()
            .InstancePerDependency();
    }

    private void RegisterContext(ContainerBuilder builder)
    {
        var options = new DbContextOptionsBuilder<InsigContext>();
        options.UseSqlServer(_connectionString);

        builder.Register(container => new InsigContext(options.Options, container.Resolve<ICurrentUserService>())).InstancePerLifetimeScope();
    }

    private void RegisterDatabaseAccess(ContainerBuilder builder)
    {
        builder
            .Register<IDbConnection>(c => new SqlConnection(_connectionString))
            .InstancePerLifetimeScope();
        builder
            .RegisterType<SqlQueryBuilder>()
            .InstancePerDependency();
    }

    private void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<CurrentUserService>().AsImplementedInterfaces().InstancePerLifetimeScope();

        RegisterTransientDependenciesAutomatically(
            builder,
            typeof(CacheService).Assembly,
            "Insig.Infrastructure.Services");
    }

    private static void RegisterControllers(ContainerBuilder builder)
    {
        RegisterTransientDependenciesAutomatically(
            builder,
            typeof(UsersController).Assembly,
            "Insig.Api.Controllers");
    }

    private static void RegisterQueries(ContainerBuilder builder)
    {
        RegisterTransientDependenciesAutomatically(
            builder,
            typeof(UsersQuery).Assembly,
            "Insig.Infrastructure.Queries");
    }

    private void RegisterRepositories(ContainerBuilder builder)
    {
        RegisterTransientDependenciesAutomatically(
            builder,
            typeof(UsersRepository).Assembly,
            "Insig.Infrastructure.Domain");
    }

    private void RegisterStorageService(ContainerBuilder builder)
    {
        builder.Register(c => new BlobServiceClient(_configuration.GetValue<string>("AzureBlobStorage")))
            .InstancePerLifetimeScope();

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var blobServiceType = environment == "Development"
            ? typeof(LocalFileStorageService)
            : typeof(AzureStorageService);

        builder.RegisterType(blobServiceType).As<IBlobService>().InstancePerLifetimeScope();
    }

    private void RegisterNotifications(ContainerBuilder builder)
    {
        RegisterTransientDependenciesAutomatically(
            builder,
            typeof(UsersHubNotifier).Assembly,
            "Insig.Notifications.Notifiers");

        builder.RegisterType<UsersHubNotifier>().As<IUsersHubNotifier>().SingleInstance();
    }

    private void RegisterEmailServices(ContainerBuilder builder)
    {
        var emailSettings = _configuration.GetSection("EmailSettings").Get<EmailSettings>();

        builder.RegisterType<MailingService>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .WithParameter(new TypedParameter(typeof(EmailSettings), emailSettings));
    }
}
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insig.Common.Auth;
using Insig.Common.Infrastructure.DateTimeProvider;
using Insig.Domain.Accesses;
using Insig.Domain.Common;
using Insig.Domain.Notifications;
using Insig.Domain.Users;
using Insig.Infrastructure.DataModel.Extensions;
using Insig.Infrastructure.DataModel.Mappings.Accesses;
using Insig.Infrastructure.DataModel.Mappings.Notifications;
using Insig.Infrastructure.DataModel.Mappings.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Insig.Infrastructure.DataModel.Context;

public class InsigContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;

    public InsigContext() { }

    public InsigContext(DbContextOptions<InsigContext> options, ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());
        builder.ApplyConfiguration(new NotificationStatusConfiguration());

        builder.ApplyUtcDateTimeConverter();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Insig"));
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        if (entries.Any())
        {
            var currentUserId = await _currentUserService.GetId();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = currentUserId;
                        entry.Entity.CreatedOn = DateTimeProvider.UtcNow();
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = currentUserId;
                        entry.Entity.UpdatedOn = DateTimeProvider.UtcNow();
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
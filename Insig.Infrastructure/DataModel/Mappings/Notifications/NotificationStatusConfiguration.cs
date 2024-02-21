using System;
using Insig.Common.Lookups;
using Insig.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Insig.Infrastructure.DataModel.Mappings.Notifications;

public class NotificationStatusConfiguration : IEntityTypeConfiguration<NotificationStatus>
{
    public void Configure(EntityTypeBuilder<NotificationStatus> builder)
    {
        builder.ToTable("NotificationStatus");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name).IsUnique();

        SeedData(builder);
    }

    private void SeedData(EntityTypeBuilder<NotificationStatus> builder)
    {
        foreach (var status in Enum.GetValues(typeof(NotificationStatusEnum)))
        {
            builder.HasData(new NotificationStatus((int)status, NotificationStatusLookup.Descriptions[(int)status]));
        }
    }
}

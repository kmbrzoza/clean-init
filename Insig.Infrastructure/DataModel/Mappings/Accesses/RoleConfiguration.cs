using System;
using Insig.Common.Auth.Lookups;
using Insig.Domain.Accesses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Insig.Infrastructure.DataModel.Mappings.Accesses;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        SeedData(builder);
    }

    private void SeedData(EntityTypeBuilder<Role> builder)
    {
        foreach (var role in Enum.GetValues(typeof(RoleEnum)))
        {
            builder.HasData(new Role((int)role, RoleLookup.Descriptions[(int)role]));
        }
    }
}

using Insig.Domain.Accesses;
using Insig.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Insig.Infrastructure.DataModel.Mappings.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Sub)
            .IsRequired();

        builder.HasIndex(u => u.Sub)
            .IsUnique();

        builder.Property(u => u.Email)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(u => u.RoleId);
    }
}

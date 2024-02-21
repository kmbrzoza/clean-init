using Insig.Domain.Notifications;
using Insig.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Insig.Infrastructure.DataModel.Mappings.Notifications;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notification");

        builder.HasKey(n => n.Id);

        builder.HasOne<NotificationStatus>().WithMany().HasForeignKey(n => n.StatusId).IsRequired();

        builder.OwnsOne(n => n.Recipient, r =>
        {
            r.Property(r => r.RecipientId).HasColumnName("RecipientId").IsRequired();
            r.HasOne<User>().WithMany().HasForeignKey(r => r.RecipientId);
        });

        builder.Navigation(n => n.Recipient).IsRequired();

        builder.OwnsOne(n => n.Message, m =>
        {
            m.Property(m => m.Title).HasColumnName("Title").IsRequired();
            m.Property(m => m.Body).HasColumnName("Body").IsRequired();
        });

        builder.Navigation(n => n.Message).IsRequired();

        builder.OwnsOne(n => n.Author, a =>
        {
            a.Property(a => a.AuthorId).HasColumnName("AuthorId");
        });

        builder.Navigation(n => n.Author).IsRequired();

        builder.OwnsOne(n => n.Action, a =>
        {
            a.Property(a => a.RedirectUrl).HasColumnName("RedirectUrl");
        });

        builder.Navigation(n => n.Action).IsRequired();
    }
}
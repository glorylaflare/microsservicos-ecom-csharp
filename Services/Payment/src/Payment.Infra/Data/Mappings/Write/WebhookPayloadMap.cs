using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Models;

namespace Payment.Infra.Data.Mappings.Write;

internal class WebhookPayloadMap : IEntityTypeConfiguration<WebhookPayload>
{
    public void Configure(EntityTypeBuilder<WebhookPayload> builder)
    {
        builder.ToTable("WebhookPayload");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Action).IsRequired().HasMaxLength(100);
        builder.Property(w => w.ApiVersion).IsRequired().HasMaxLength(100);
        builder.OwnsOne(w => w.Data, d =>
        {
            d.Property(d => d.Id).IsRequired().HasColumnName("PaymentId");
        });
        builder.Property(w => w.DateCreated).IsRequired().HasColumnType("datetime2");
        builder.Property(w => w.LiveMode).IsRequired();
        builder.Property(w => w.Type).IsRequired().HasMaxLength(100);
        builder.Property(w => w.UserId).IsRequired().HasMaxLength(100);
    }
};

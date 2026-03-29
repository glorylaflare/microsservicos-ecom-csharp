using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Models;

namespace Payment.Infra.Data.Mappings.Write;

public class WebhookEventMap : IEntityTypeConfiguration<WebhookEvent>
{
    public void Configure(EntityTypeBuilder<WebhookEvent> builder)
    {
        builder.ToTable("WebhookEvents");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Provider).IsRequired().HasMaxLength(50);
        builder.Property(w => w.EventId).IsRequired().HasMaxLength(100);
        builder.HasIndex(w => w.EventId).IsUnique();
        builder.HasOne(w => w.Payload)
           .WithOne()
           .HasForeignKey<WebhookPayload>("WebhookEventId")
           .OnDelete(DeleteBehavior.Cascade);
        builder.Property(w => w.Status).IsRequired().HasConversion<string>(); 
        builder.Property(w => w.CreatedAt).IsRequired().HasColumnType("datetime2");
        builder.Property(w => w.UpdatedAt).HasColumnType("datetime2");
    }
}
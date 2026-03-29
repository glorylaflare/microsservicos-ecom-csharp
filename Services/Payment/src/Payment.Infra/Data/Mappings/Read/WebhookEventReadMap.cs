using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Infra.Data.Mappings.Read;

public class WebhookEventReadMap : IEntityTypeConfiguration<WebhookEventReadModel>
{
    public void Configure(EntityTypeBuilder<WebhookEventReadModel> builder)
    {
        builder.ToView("vw_WebhookEvents");
        builder.HasNoKey();
        builder.Property(x => x.Provider);
        builder.Property(x => x.EventId);
        builder.Property(x => x.Action);
        builder.Property(x => x.Status);
        builder.Property(x => x.ApiVersion);
        builder.Property(x => x.PaymentId);
        builder.Property(x => x.DateCreated);
        builder.Property(x => x.Id);
        builder.Property(x => x.LiveMode);
        builder.Property(x => x.Type);
        builder.Property(x => x.UserId);
        builder.Property(x => x.Status);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
    }
}
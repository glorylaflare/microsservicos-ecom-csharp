using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Payment.Infra.Data.Mappings.Read;

public class PaymentReadMap : IEntityTypeConfiguration<PaymentReadModel>
{
    public void Configure(EntityTypeBuilder<PaymentReadModel> builder)
    {
        builder.ToView("vw_Payments");
        builder.HasNoKey();
        builder.Property(x => x.Id);
        builder.Property(x => x.OrderId);
        builder.Property(x => x.Amount);
        builder.Property(x => x.Status);
        builder.Property(x => x.CheckoutUrl);
        builder.Property(x => x.MercadoPagoPreference);
        builder.Property(x => x.ExpirationDate);
        builder.Property(x => x.CreatedAt);
        builder.Property(x => x.UpdatedAt);
    }
}
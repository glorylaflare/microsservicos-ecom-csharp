using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Payment.Infra.Data.Mappings.Write;

public class PaymentMap : IEntityTypeConfiguration<Domain.Models.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.Status).IsRequired().HasConversion<string>();
        builder.Property(p => p.CheckoutUrl).IsRequired(false).HasMaxLength(500);
        builder.Property(p => p.MercadoPagoPreference).IsRequired().HasMaxLength(100);
        builder.Property(p => p.MercadoPagoPaymentId).HasColumnType("bigint").IsRequired();
        builder.Property(p => p.ExpirationDate).IsRequired(false).HasColumnType("datetime");
        builder.Property(p => p.CreatedAt).IsRequired().HasColumnType("datetime");
        builder.Property(p => p.UpdatedAt).HasColumnType("datetime");
    }
}
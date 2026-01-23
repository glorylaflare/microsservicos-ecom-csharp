using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Order.Infra.Data.Mappings.Write;
public class OrderMap : IEntityTypeConfiguration<Domain.Models.Order>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.OwnsMany(o => o.Items, oi =>
        {
            oi.WithOwner().HasForeignKey("OrderId");
            oi.Property<int>("Id");
            oi.HasKey("Id");
            oi.Property(i => i.ProductId).IsRequired();
            oi.Property(i => i.Quantity).IsRequired();
        });
        builder.Property(o => o.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired().HasColumnType("datetime");
        builder.Property(o => o.UpdatedAt).HasColumnType("datetime");
    }
}
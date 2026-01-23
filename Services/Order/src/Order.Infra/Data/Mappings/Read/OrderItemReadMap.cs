using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
namespace Order.Infra.Data.Mappings.Read;
public class OrderItemReadMap : IEntityTypeConfiguration<OrderItemReadModel>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrderItemReadModel> builder)
    {
        builder.ToView("vw_OrderItems");
        builder.HasKey(x => new { x.OrderId, x.ProductId });
        builder.Property(x => x.OrderId);
        builder.Property(x => x.ProductId);
        builder.Property(x => x.Quantity);
    }
}
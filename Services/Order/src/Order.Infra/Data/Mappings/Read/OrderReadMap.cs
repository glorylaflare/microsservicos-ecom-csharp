using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Order.Infra.Data.Mappings.Read;

public class OrderReadMap : IEntityTypeConfiguration<OrderReadModel>
{
    public void Configure(EntityTypeBuilder<OrderReadModel> builder)
    {
        builder.ToView("vw_Orders");
        builder.HasNoKey();
        builder.Property(p => p.Id);
        builder.Property(o => o.Status);
        builder.Property(o => o.TotalAmount);
        builder.Property(o => o.CreatedAt);
        builder.Property(o => o.UpdatedAt);
        builder.Ignore(o => o.Items);
    }
}
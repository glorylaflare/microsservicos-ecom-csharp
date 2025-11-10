using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Stock.Infra.Data.Mappings.Read;

public class ProductReadMap : IEntityTypeConfiguration<ProductReadModel>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ProductReadModel> builder)
    {
        builder.ToView("vw_Products");
        builder.HasNoKey();

        builder.Property(p => p.Id);
        builder.Property(p => p.Name);
        builder.Property(p => p.Description);
        builder.Property(p => p.Price);
        builder.Property(p => p.StockQuantity);
        builder.Property(p => p.CreatedAt);
        builder.Property(p => p.UpdatedAt);
    }
}

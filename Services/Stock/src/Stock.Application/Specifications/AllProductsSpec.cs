using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Specifications;

namespace Stock.Application.Specifications;

public class AllProductsSpec : Specification<ProductReadModel, ProductReadModel>
{
    public AllProductsSpec(int skip, int take) : base(x => 
        new ProductReadModel
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            StockQuantity = x.StockQuantity,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        })
    {
        ApplyPaging(skip, take);
    }
}

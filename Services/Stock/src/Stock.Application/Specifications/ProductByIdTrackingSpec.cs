using BuildingBlocks.Infra.Specifications;
using Stock.Domain.Models;

namespace Stock.Application.Specifications;

public class ProductByIdTrackingSpec : Specification<Product>
{
    public ProductByIdTrackingSpec(int id)
    {
        AddCriteria(p => p.Id == id);
        EnableTracking();
    }
}

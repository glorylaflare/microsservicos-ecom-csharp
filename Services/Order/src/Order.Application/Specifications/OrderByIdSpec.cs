using BuildingBlocks.Infra.Specifications;

namespace Order.Application.Specifications;

public class OrderByIdSpec : Specification<Domain.Models.Order>
{
    public OrderByIdSpec(int id)
    {
        AddCriteria(o => o.Id == id);
        EnableTracking();
    }
}
using BuildingBlocks.Infra.Specifications;

namespace Payment.Application.Specifications;

public class PaymentByOrderIdSpec : Specification<Domain.Models.Payment>
{
    public PaymentByOrderIdSpec(int orderId)
    {
        AddCriteria(x => x.OrderId == orderId);
        EnableTracking();
    }
}

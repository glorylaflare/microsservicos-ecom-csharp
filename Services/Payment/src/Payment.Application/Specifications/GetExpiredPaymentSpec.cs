using BuildingBlocks.Infra.Specifications;
using Payment.Domain.Models;

namespace Payment.Application.Specifications;

public class GetExpiredPaymentSpec : Specification<Domain.Models.Payment>
{
    public GetExpiredPaymentSpec(DateTime currentTime) 
    {
        AddCriteria(p => p.Status == PaymentStatus.Pending && p.ExpirationDate <= currentTime);
        EnableTracking();
    }
}

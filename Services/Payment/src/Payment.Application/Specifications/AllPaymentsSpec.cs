using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Specifications;

namespace Payment.Application.Specifications;

public class AllPaymentsSpec : Specification<PaymentReadModel, PaymentReadModel>
{
    public AllPaymentsSpec(int skip, int take) : base(x =>
        new PaymentReadModel
        {
            Id = x.Id,
            OrderId = x.OrderId,
            Amount = x.Amount,
            Status = x.Status,
            CheckoutUrl = x.CheckoutUrl,
            MercadoPagoPreference = x.MercadoPagoPreference,
            MercadoPagoPaymentId = x.MercadoPagoPaymentId,
            ExpirationDate = x.ExpirationDate,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        })
    {
        ApplyPaging(skip, take);
    }
}

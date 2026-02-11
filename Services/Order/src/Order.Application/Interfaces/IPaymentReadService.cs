using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Interfaces;

public interface IPaymentReadService
{
    Task<PaymentReadModel?> GetByIdAsync(int orderId);
}

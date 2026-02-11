using BuildingBlocks.Infra.MongoReadModels;
using MongoDB.Driver;
using Order.Application.Interfaces;
using Order.Infra.Data.Context;

namespace Order.Infra.Data.Services;

public class PaymentReadService : IPaymentReadService
{
    private readonly IMongoCollection<PaymentReadModel> _payments;

    public PaymentReadService(ReadDbContext context)
    {
        _payments = context.Payments;
    }

    public Task<PaymentReadModel?> GetByIdAsync(int orderId)
    {
        return _payments.Find(p => p.OrderId == orderId).FirstOrDefaultAsync()!;
    }
}

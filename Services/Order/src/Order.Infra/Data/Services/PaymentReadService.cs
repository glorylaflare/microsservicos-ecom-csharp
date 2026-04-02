using BuildingBlocks.Infra.MongoReadModels;
using MongoDB.Driver;
using Order.Application.Interfaces;
using Order.Infra.Data.Context.Read;

namespace Order.Infra.Data.Services;

public class PaymentReadService : IPaymentReadService
{
    private readonly IMongoCollection<PaymentReadModel> _payments;

    public PaymentReadService(ReadDbContext context)
    {
        _payments = context.Payments;
    }

    public async Task<PaymentReadModel?> GetByIdAsync(int orderId)
        => await _payments.Find(p => p.OrderId == orderId).FirstOrDefaultAsync();
}

using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Repositories;
using Payment.Application.Interfaces;
using Payment.Infra.Data.Context.Read;
namespace Payment.Infra.Data.Services;

public class PaymentReadService : Repository<PaymentReadModel>, IPaymentReadService
{
    public PaymentReadService(ReadDbContext context) : base(context) { }
}
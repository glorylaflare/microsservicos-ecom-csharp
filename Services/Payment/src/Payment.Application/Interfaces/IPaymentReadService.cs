using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Infra.ReadModels;

namespace Payment.Application.Interfaces;

public interface IPaymentReadService : IRepository<PaymentReadModel>
{ }
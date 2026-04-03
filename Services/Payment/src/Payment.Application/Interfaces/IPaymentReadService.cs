using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Infra.ReadModels;

namespace Payment.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IPaymentReadService.
/// </summary>
public interface IPaymentReadService : IRepository<PaymentReadModel>
{ }

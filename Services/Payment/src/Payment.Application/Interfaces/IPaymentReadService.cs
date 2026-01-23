using BuildingBlocks.Infra.ReadModels;
namespace Payment.Application.Interfaces;
public interface IPaymentReadService
{
    Task<PaymentReadModel?> GetByIdAsync(int paymentId);
    Task<IEnumerable<PaymentReadModel>> GetAllAsync();
}
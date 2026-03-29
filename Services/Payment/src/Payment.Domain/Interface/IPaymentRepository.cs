using BuildingBlocks.Infra.Interfaces;

namespace Payment.Domain.Interface;

public interface IPaymentRepository : IRepository<Models.Payment>
{
    Task<int> SetExpiredPaymentsAsync(DateTime currentTime);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
using BuildingBlocks.Infra.Interfaces;

namespace Order.Domain.Interfaces;

public interface IOrderRepository : IRepository<Models.Order>
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
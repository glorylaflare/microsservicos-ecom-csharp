using BuildingBlocks.Infra.Interfaces;
using Stock.Domain.Models;

namespace Stock.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
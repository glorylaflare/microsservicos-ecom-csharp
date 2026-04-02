using BuildingBlocks.Infra.Repositories;
using Order.Domain.Interfaces;
using Order.Infra.Data.Context.Write;
namespace Order.Infra.Data.Repositories;

public class OrderRepository : Repository<Domain.Models.Order>, IOrderRepository
{
    public OrderRepository(WriteDbContext context) : base(context) { }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}
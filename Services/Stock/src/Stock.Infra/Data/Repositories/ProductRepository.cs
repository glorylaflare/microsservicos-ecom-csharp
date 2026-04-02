using BuildingBlocks.Infra.Repositories;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;
using Stock.Infra.Data.Context.Write;

namespace Stock.Infra.Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(WriteDbContext context) : base(context) { }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}
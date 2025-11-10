using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using Stock.Application.Interfaces;
using Stock.Infra.Data.Context;

namespace Stock.Infra.Data.Services;

public class ProductReadService : IProductReadService
{
    private readonly DbSet<ProductReadModel> _products;

    public ProductReadService(ReadDbContext context)
    {
        _products = context.Products;
    }

    public async Task<IEnumerable<ProductReadModel>> GetAllAsync()
    {
        return await _products.Select(p => new ProductReadModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProductReadModel?> GetByIdAsync(int productId)
    {
        return await _products.Where(p => p.Id == productId)
            .Select(p => new ProductReadModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Serilog;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;
using Stock.Infra.Data.Context;

namespace Stock.Infra.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StockDbContext _context;
    private readonly ILogger _logger;

    public ProductRepository(StockDbContext context)
    {
        _context = context;
        _logger = Log.ForContext<ProductRepository>();
    }

    public async Task AddProductAsync(Product product)
    {
        _logger.Debug("Adding a new product with ID {ProductId}", product.Id);
        await _context.Products.AddAsync(product);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        _logger.Debug("Fetching all products from the database");
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        _logger.Debug("Fetching product with ID {ProductId}", productId);
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task SaveChangesAsync()
    {
        _logger.Debug("Saving changes to the database");
        await _context.SaveChangesAsync();
        _logger.Debug("Changes saved successfully");
    }
}

using Microsoft.EntityFrameworkCore;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;
using Stock.Infra.Data.Context;

namespace Stock.Infra.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StockDbContext _context;

    public ProductRepository(StockDbContext context)
    {
        _context = context;
    }

    public async Task AddProductAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

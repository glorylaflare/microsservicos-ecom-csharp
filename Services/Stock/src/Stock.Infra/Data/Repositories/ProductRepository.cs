using Microsoft.EntityFrameworkCore;
using Stock.Domain.Interfaces;
using Stock.Domain.Models;
using Stock.Infra.Data.Context;

namespace Stock.Infra.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DbSet<Product> _products;
    private readonly WriteDbContext _context;

    public ProductRepository(WriteDbContext context)
    {
        _products = context.Products;
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int productId) =>
        await _products.FirstOrDefaultAsync(p => p.Id == productId);

    public async Task AddAsync(Product product) => await _products.AddAsync(product);

    public void Update(Product product) => _products.Update(product);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

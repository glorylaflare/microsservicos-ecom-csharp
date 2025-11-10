using Stock.Domain.Models;

namespace Stock.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int productId);
    Task AddAsync(Product product);
    void Update(Product product);
    Task SaveChangesAsync();
}

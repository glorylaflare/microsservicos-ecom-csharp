using Stock.Domain.Models;

namespace Stock.Domain.Interfaces;

public interface IProductRepository
{
    Task AddProductAsync(Product product);
    Task<Product?> GetProductByIdAsync(int productId);
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task SaveChangesAsync();
}

using BuildingBlocks.Infra.ReadModels;
namespace Stock.Application.Interfaces;

public interface IProductReadService
{
    Task<ProductReadModel?> GetByIdAsync(int productId);
    Task<IEnumerable<ProductReadModel>> GetAllAsync();
}
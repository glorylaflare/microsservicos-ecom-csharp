using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Interfaces;

public interface IOrderReadService
{
    Task<OrderReadModel?> GetByIdAsync(int orderId);
    Task<IReadOnlyList<OrderReadModel>> GetAllAsync();
}
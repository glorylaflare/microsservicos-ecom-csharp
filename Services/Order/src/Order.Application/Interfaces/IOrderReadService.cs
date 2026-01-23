using BuildingBlocks.Infra.ReadModels;
namespace Order.Application.Interfaces;

public interface IOrderReadService
{
    Task<OrderReadModel?> GetByIdAsync(int orderId);
    Task<IEnumerable<OrderReadModel>> GetAllAsync();
}
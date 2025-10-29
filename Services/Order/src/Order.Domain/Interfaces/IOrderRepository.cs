namespace Order.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddOrderAsync(Models.Order order);
    Task<Models.Order?> GetOrderByIdAsync(int orderId);
    Task SaveChangesAsync();
}

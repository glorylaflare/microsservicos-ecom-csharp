namespace Order.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Models.Order?> GetByIdAsync(int orderId);
    Task AddAsync(Models.Order order);
    void Update(Models.Order order);
    Task SaveChangesAsync();
}

using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces;
using Order.Infra.Data.Context;

namespace Order.Infra.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DbSet<Domain.Models.Order> _orders;
    private readonly WriteDbContext _context;

    public OrderRepository(WriteDbContext context)
    {
        _orders = context.Orders;
        _context = context;
    }

    public async Task<Domain.Models.Order?> GetByIdAsync(int orderId) =>
        await _orders.FirstOrDefaultAsync(o => o.Id == orderId);

    public async Task AddAsync(Domain.Models.Order order) => await _orders.AddAsync(order);

    public void Update(Domain.Models.Order order) => _orders.Update(order);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

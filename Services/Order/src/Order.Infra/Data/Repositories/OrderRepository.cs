using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces;
using Order.Infra.Data.Context;

namespace Order.Infra.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task AddOrderAsync(Domain.Models.Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Domain.Models.Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces;
using Order.Infra.Data.Context;
using Serilog;

namespace Order.Infra.Data.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;
    private readonly ILogger _logger;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
        _logger = Log.ForContext<OrderRepository>();
    }

    public async Task AddOrderAsync(Domain.Models.Order order)
    {
        _logger.Debug("Adding a new order with ID {OrderId}", order.Id);
        await _context.Orders.AddAsync(order);
    }

    public async Task<Domain.Models.Order?> GetOrderByIdAsync(int orderId)
    {
        _logger.Debug("Fetching order with ID {OrderId}", orderId);
        return await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task SaveChangesAsync()
    {
        _logger.Debug("Saving changes to the database");
        await _context.SaveChangesAsync();
        _logger.Debug("Changes saved successfully");
    }
}

using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using Order.Application.Interfaces;
using Order.Infra.Data.Context;

namespace Order.Infra.Data.Services;

public class OrderReadService : IOrderReadService
{
    private readonly DbSet<OrderReadModel> _orders;
    private readonly DbSet<OrderItemReadModel> _orderItems;

    public OrderReadService(ReadDbContext context)
    {
        _orders = context.Orders;
        _orderItems = context.OrderItems;
    }

    public async Task<OrderReadModel?> GetByIdAsync(int orderId)
    {
        var result = await _orders.Where(o => o.Id == orderId)
            .Select(o => new OrderReadModel
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (result is null) return null;

        result.Items = await _orderItems.Where(oi => oi.OrderId == orderId)
            .Select(oi => new OrderItemReadModel
            {
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity
            })
            .AsNoTracking()
            .ToListAsync();

        return result;
    }

    public async Task<IEnumerable<OrderReadModel>> GetAllAsync()
    {
        var orders = await _orders.Select(o => new OrderReadModel 
            { 
                Id = o.Id, 
                Status = o.Status, 
                TotalAmount = o.TotalAmount, 
                CreatedAt = o.CreatedAt, 
                UpdatedAt = o.UpdatedAt 
            })
            .AsNoTracking()
            .ToListAsync();

        var items = await _orderItems.Select(oi => new OrderItemReadModel 
            { 
                OrderId = oi.OrderId, 
                ProductId = oi.ProductId, 
                Quantity = oi.Quantity 
            })
            .AsNoTracking()
            .ToListAsync();

        foreach (var order in orders)
        {
            order.Items = items.Where(i => i.OrderId == order.Id)
                .ToList();
        }

        return orders;
    }
}

using BuildingBlocks.Infra.MongoReadModels;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Order.Application.Interfaces;
using Order.Infra.Data.Context;
namespace Order.Infra.Data.Services;

public class OrderReadService : IOrderReadService
{
    private readonly IMongoCollection<OrderReadModel> _orders;

    public OrderReadService(ReadDbContext context)
    {
        _orders = context.Orders;
    }

    public async Task<OrderReadModel?> GetByIdAsync(int orderId)
    {
        return await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<OrderReadModel>> GetAllAsync()
    {
        return await _orders.Find(_ => true).ToListAsync();
    }
}
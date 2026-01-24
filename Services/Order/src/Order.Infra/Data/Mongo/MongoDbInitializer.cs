using BuildingBlocks.Infra.MongoReadModels;
using MongoDB.Driver;
using Order.Infra.Data.Context;
using Serilog;

namespace Order.Infra.Data.Mongo;

public class MongoDbInitializer
{
    private readonly ReadDbContext _context;
    private readonly ILogger _logger;

    public MongoDbInitializer(ReadDbContext context)
    {
        _context = context;
        _logger = Log.ForContext<MongoDbInitializer>();
    }

    public async Task InitializeAsync()
    {
        var collections = await _context.Database.ListCollectionNames().ToListAsync();

        if (!collections.Contains("orders"))
        {
            await _context.Database.CreateCollectionAsync("orders");
            _logger.Information("[INFO] Created 'orders' collection in MongoDB.");
        }

        await _context.Orders.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<OrderReadModel>(
                Builders<OrderReadModel>.IndexKeys.Ascending(o => o.CreatedAt)
            )
        });
    }
}

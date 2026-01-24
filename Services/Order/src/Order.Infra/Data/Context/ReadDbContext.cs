using BuildingBlocks.Infra.MongoReadModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Order.Infra.Data.Context;

public class ReadDbContext
{
    private readonly IMongoDatabase _database;

    public ReadDbContext(IOptions<BuildingBlocks.SharedKernel.Config.MongoDatabaseSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoDatabase Database => _database;

    public IMongoCollection<OrderReadModel> Orders =>         
        _database.GetCollection<OrderReadModel>("orders");
}
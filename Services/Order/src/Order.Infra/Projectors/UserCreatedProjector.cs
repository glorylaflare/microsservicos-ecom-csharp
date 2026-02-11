using BuildingBlocks.Contracts.MongoEvents;
using BuildingBlocks.Infra.MongoReadModels;
using BuildingBlocks.Messaging;
using MongoDB.Driver;
using Order.Infra.Data.Context;
using Serilog;

namespace Order.Infra.Projectors;

public class UserCreatedProjector : IIntegrationEventHandler<UserCreatedEvent>
{
    private readonly IMongoCollection<UserReadModel> _users;
    private readonly ILogger _logger;

    public UserCreatedProjector(ReadDbContext context)
    {
        _users = context.Users;
        _logger = Log.ForContext<UserCreatedProjector>();
    }

    public async Task HandleAsync(UserCreatedEvent @event)
    {
        _logger.Information("Projecting {Event} to read model", nameof(UserCreatedEvent));

        await _users.InsertOneAsync(new UserReadModel
        {
            Id = @event.Data.Id,
            Username = @event.Data.Username,
            Email = @event.Data.Email
        });

        _logger.Information("Projected {Event} to read model", nameof(UserCreatedEvent));
    }
}

using BuildingBlocks.Infra.MongoReadModels;
using MongoDB.Driver;
using Order.Application.Interfaces;
using Order.Infra.Data.Context;

namespace Order.Infra.Data.Services;

public class UserReadService : IUserReadService
{
    private readonly IMongoCollection<UserReadModel> _users;

    public UserReadService(ReadDbContext context)
    {
        _users = context.Users;
    }

    public Task<UserReadModel?> GetByIdAsync(string userId)
    {
        return _users.Find(x => x.Id == userId).FirstOrDefaultAsync()!;
    }
}

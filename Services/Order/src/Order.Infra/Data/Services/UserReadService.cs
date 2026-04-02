using BuildingBlocks.Infra.MongoReadModels;
using MongoDB.Driver;
using Order.Application.Interfaces;
using Order.Infra.Data.Context.Read;

namespace Order.Infra.Data.Services;

public class UserReadService : IUserReadService
{
    private readonly IMongoCollection<UserReadModel> _users;

    public UserReadService(ReadDbContext context)
    {
        _users = context.Users;
    }

    public async Task<UserReadModel?> GetByIdAsync(string userId)
        => await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
}

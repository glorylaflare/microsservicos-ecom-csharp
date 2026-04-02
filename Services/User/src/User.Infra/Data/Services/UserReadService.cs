using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Repositories;
using User.Application.Interfaces;
using User.Infra.Data.Context.Read;

namespace User.Infra.Data.Services;

public class UserReadService : Repository<UserReadModel>, IUserReadService
{
    public UserReadService(ReadDbContext context) : base(context) { }
}
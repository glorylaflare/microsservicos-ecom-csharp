using BuildingBlocks.Infra.Interfaces;

namespace User.Domain.Interfaces;

public interface IUserRepository : IRepository<Models.User>
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
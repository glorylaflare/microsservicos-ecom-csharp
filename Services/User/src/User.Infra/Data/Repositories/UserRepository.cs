using BuildingBlocks.Infra.Repositories;
using User.Domain.Interfaces;
using User.Infra.Data.Context.Write;

namespace User.Infra.Data.Repositories;

public class UserRepository : Repository<Domain.Models.User>, IUserRepository
{
    public UserRepository(WriteDbContext context) : base(context) { }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}
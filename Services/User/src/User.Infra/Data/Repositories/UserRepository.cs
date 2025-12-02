using Microsoft.EntityFrameworkCore;
using User.Domain.Interfaces;
using User.Infra.Data.Context;

namespace User.Infra.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbSet<Domain.Models.User> _users;
    private readonly WriteDbContext _context;

    public UserRepository(WriteDbContext context)
    {
        _context = context;
        _users = context.Users;
    }

    public async Task AddAsync(Domain.Models.User user) => await _users.AddAsync(user);

    public async Task<Domain.Models.User?> GetByIdAsync(int userId) => 
        await _users.FirstOrDefaultAsync(u => u.Id == userId);

    public async Task<Domain.Models.User?> GetByEmailAsync(string email) =>
        await _users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

using Microsoft.EntityFrameworkCore;
using User.Domain.Interfaces;
using User.Infra.Data.Context;

namespace User.Infra.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task RegisterUserAsync(Domain.Models.User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<IEnumerable<Domain.Models.User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<Domain.Models.User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<Domain.Models.User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

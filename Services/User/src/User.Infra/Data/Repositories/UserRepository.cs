using Microsoft.EntityFrameworkCore;
using Serilog;
using User.Domain.Interfaces;
using User.Infra.Data.Context;

namespace User.Infra.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;
    private readonly ILogger _logger;

    public UserRepository(UserDbContext context)
    {
        _context = context;
        _logger = Log.ForContext<UserRepository>();
    }

    public async Task RegisterUserAsync(Domain.Models.User user)
    {
        _logger.Debug("Registering a new user with ID {UserId}", user.Id);
        await _context.Users.AddAsync(user);
    }

    public async Task<IEnumerable<Domain.Models.User>> GetAllUsersAsync()
    {
        _logger.Debug("Fetching all users from the database");
        return await _context.Users.ToListAsync();
    }

    public async Task<Domain.Models.User?> GetUserByIdAsync(int userId)
    {
        _logger.Debug("Fetching user with ID {UserId}", userId);
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<Domain.Models.User?> GetUserByEmailAsync(string email)
    {
        _logger.Debug("Fetching user with email {Email}", email);
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task SaveChangesAsync()
    {
        _logger.Debug("Saving changes to the database");
        await _context.SaveChangesAsync();
        _logger.Debug("Changes saved successfully");
    }
}

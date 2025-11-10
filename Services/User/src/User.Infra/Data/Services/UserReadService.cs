using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using User.Application.Interfaces;
using User.Infra.Data.Context;

namespace User.Infra.Data.Services;

public class UserReadService : IUserReadService
{
    private readonly DbSet<UserReadModel> _users;

    public UserReadService(ReadDbContext context)
    {
        _users = context.Users;
    }

    public async Task<UserReadModel?> GetByEmailAsync(string email)
    {
        return await _users.Where(u => u.Email == email)
            .Select(u => new UserReadModel
            {
                Id = u.Id,
                Username = u.Username,  
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<UserReadModel?> GetByIdAsync(int id)
    {
        return await _users.Where(u => u.Id == id)
            .Select(u => new UserReadModel
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserReadModel>> GetAllAsync()
    {
        return await _users.Select(u => new UserReadModel
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .AsNoTracking()
            .ToListAsync();
    }
}

namespace User.Domain.Interfaces;

public interface IUserRepository
{
    Task AddAsync(Models.User user);
    Task<Models.User?> GetByIdAsync(int userId);
    Task SaveChangesAsync();
}

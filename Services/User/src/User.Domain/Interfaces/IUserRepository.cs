namespace User.Domain.Interfaces;

public interface IUserRepository
{
    Task RegisterUserAsync(Models.User user);
    Task<Models.User?> GetUserByEmailAsync(string email);
    Task<Models.User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<Models.User>> GetAllUsersAsync();
    Task SaveChangesAsync();
}

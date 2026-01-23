using BuildingBlocks.Infra.ReadModels;
namespace User.Application.Interfaces;

public interface IUserReadService
{
    Task<UserReadModel?> GetByEmailAsync(string email);
    Task<UserReadModel?> GetByIdAsync(int id);
    Task<IEnumerable<UserReadModel>> GetAllAsync();
}
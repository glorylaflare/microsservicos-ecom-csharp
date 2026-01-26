using BuildingBlocks.Infra.MongoReadModels;

namespace Order.Application.Interfaces
{
    public interface IUserReadService
    {
        Task<UserReadModel?> GetByIdAsync(string userId);
    }
}

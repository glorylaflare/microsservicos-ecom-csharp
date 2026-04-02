using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Infra.ReadModels;

namespace User.Application.Interfaces;

public interface IUserReadService : IRepository<UserReadModel>
{ }
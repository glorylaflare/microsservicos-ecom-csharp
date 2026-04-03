using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Infra.ReadModels;

namespace User.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IUserReadService.
/// </summary>
public interface IUserReadService : IRepository<UserReadModel>
{ }

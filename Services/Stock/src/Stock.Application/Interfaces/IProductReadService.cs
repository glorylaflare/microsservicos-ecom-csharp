using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Infra.ReadModels;

namespace Stock.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IProductReadService.
/// </summary>
public interface IProductReadService : IRepository<ProductReadModel>
{ }

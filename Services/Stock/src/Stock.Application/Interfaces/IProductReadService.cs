using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Infra.ReadModels;

namespace Stock.Application.Interfaces;

public interface IProductReadService : IRepository<ProductReadModel>
{ }
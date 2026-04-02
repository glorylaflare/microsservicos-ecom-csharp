using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Repositories;
using Stock.Application.Interfaces;
using Stock.Infra.Data.Context.Read;
namespace Stock.Infra.Data.Services;

public class ProductReadService : Repository<ProductReadModel>, IProductReadService
{
    public ProductReadService(ReadDbContext context) : base(context) { }
}
using BuildingBlocks.Infra.ReadModels;
using BuildingBlocks.Infra.Repositories;
using Payment.Application.Interfaces;
using Payment.Infra.Data.Context.Read;

namespace Payment.Infra.Data.Services;

public class WebhookEventReadService : Repository<WebhookEventReadModel>, IWebhookEventReadService
{
    public WebhookEventReadService(ReadDbContext context) : base(context) { }
}

using BuildingBlocks.Infra.Interfaces;
using Payment.Domain.Models;

namespace Payment.Domain.Interfaces;

public interface IWebhookRepository : IRepository<WebhookEvent>
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

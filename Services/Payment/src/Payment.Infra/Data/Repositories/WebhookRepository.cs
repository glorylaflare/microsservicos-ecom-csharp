using BuildingBlocks.Infra.Repositories;
using Payment.Domain.Interfaces;
using Payment.Domain.Models;
using Payment.Infra.Data.Context.Write;

namespace Payment.Infra.Data.Repositories;

public class WebhookRepository : Repository<WebhookEvent>, IWebhookRepository
{
    public WebhookRepository(WriteDbContext context) : base(context) { }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) => await _context.SaveChangesAsync(cancellationToken);
}

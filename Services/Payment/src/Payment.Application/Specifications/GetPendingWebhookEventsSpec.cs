using BuildingBlocks.Infra.Specifications;
using Payment.Domain.Models;

namespace Payment.Application.Specifications;

public class GetPendingWebhookEventsSpec : Specification<WebhookEvent>
{
    public GetPendingWebhookEventsSpec()
    {
        AddCriteria(w => w.Status == WebhookStatus.Pending);
        AddIncludes(w => w.Payload);
        EnableTracking();
    }
}

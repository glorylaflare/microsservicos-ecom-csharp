using BuildingBlocks.Infra.Interfaces;
using BuildingBlocks.Infra.ReadModels;

namespace Payment.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IWebhookEventReadService.
/// </summary>
public interface IWebhookEventReadService : IRepository<WebhookEventReadModel>
{ }


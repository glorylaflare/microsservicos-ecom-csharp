using BuildingBlocks.Infra.Interfaces;
using Payment.Domain.Models;

namespace Payment.Domain.Interfaces;

/// <summary>
/// Define o contrato da interface IWebhookRepository.
/// </summary>
public interface IWebhookRepository : IRepository<WebhookEvent>
{
    /// <summary>
    /// Executa o contrato do metodo SaveChangesAsync.
    /// </summary>
    /// <param name="cancellationToken">Parametro do metodo SaveChangesAsync.</param>
    /// <returns>Resultado da execucao do metodo SaveChangesAsync.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}


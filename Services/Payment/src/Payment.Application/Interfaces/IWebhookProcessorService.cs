namespace Payment.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IWebhookProcessorService.
/// </summary>
public interface IWebhookProcessorService
{
    /// <summary>
    /// Executa o contrato do metodo ProcessWebhookAsync.
    /// </summary>
    /// <returns>Resultado da execucao do metodo ProcessWebhookAsync.</returns>
    Task ProcessWebhookAsync();
}


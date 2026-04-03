using Notification.Domain.Models;

namespace Notification.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IEmailSender.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Executa o contrato do metodo SendAsync.
    /// </summary>
    /// <param name="message">Parametro do metodo SendAsync.</param>
    /// <returns>Resultado da execucao do metodo SendAsync.</returns>
    Task SendAsync(Message message);
}


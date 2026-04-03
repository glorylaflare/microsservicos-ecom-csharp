namespace Order.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IOrderEmailPublisher.
/// </summary>
public interface IOrderEmailPublisher
{
    /// <summary>
    /// Executa o contrato do metodo PublishPending.
    /// </summary>
    /// <param name="order">Parametro do metodo PublishPending.</param>
    /// <param name="checkoutUrl">Parametro do metodo PublishPending.</param>
    /// <returns>Resultado da execucao do metodo PublishPending.</returns>
    Task PublishPending(Domain.Models.Order order, string checkoutUrl);
    /// <summary>
    /// Executa o contrato do metodo PublishCompleted.
    /// </summary>
    /// <param name="order">Parametro do metodo PublishCompleted.</param>
    /// <returns>Resultado da execucao do metodo PublishCompleted.</returns>
    Task PublishCompleted(Domain.Models.Order order);
    /// <summary>
    /// Executa o contrato do metodo PublishFailed.
    /// </summary>
    /// <param name="order">Parametro do metodo PublishFailed.</param>
    /// <returns>Resultado da execucao do metodo PublishFailed.</returns>
    Task PublishFailed(Domain.Models.Order order);
}


using BuildingBlocks.Infra.Interfaces;

namespace Payment.Domain.Interface;

/// <summary>
/// Define o contrato da interface IPaymentRepository.
/// </summary>
public interface IPaymentRepository : IRepository<Models.Payment>
{
    /// <summary>
    /// Executa o contrato do metodo SetExpiredPaymentsAsync.
    /// </summary>
    /// <param name="currentTime">Parametro do metodo SetExpiredPaymentsAsync.</param>
    /// <returns>Resultado da execucao do metodo SetExpiredPaymentsAsync.</returns>
    Task<int> SetExpiredPaymentsAsync(DateTime currentTime);
    /// <summary>
    /// Executa o contrato do metodo SaveChangesAsync.
    /// </summary>
    /// <param name="cancellationToken">Parametro do metodo SaveChangesAsync.</param>
    /// <returns>Resultado da execucao do metodo SaveChangesAsync.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

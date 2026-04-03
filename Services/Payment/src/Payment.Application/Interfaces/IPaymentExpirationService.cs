namespace Payment.Application.Interfaces;

/// <summary>
/// Define o contrato da interface IPaymentExpirationService.
/// </summary>
public interface IPaymentExpirationService
{
    /// <summary>
    /// Executa o contrato do metodo ProcessExpiredPaymentsAsync.
    /// </summary>
    /// <returns>Resultado da execucao do metodo ProcessExpiredPaymentsAsync.</returns>
    Task ProcessExpiredPaymentsAsync();
}


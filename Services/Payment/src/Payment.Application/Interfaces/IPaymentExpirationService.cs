namespace Payment.Application.Interfaces;

public interface IPaymentExpirationService
{
    Task ProcessExpiredPaymentsAsync();
}

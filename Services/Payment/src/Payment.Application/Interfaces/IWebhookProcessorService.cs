namespace Payment.Application.Interfaces;

public interface IWebhookProcessorService
{
    Task ProcessWebhookAsync();
}

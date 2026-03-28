namespace Payment.Infra.Configurations;

public class MercadoPagoSettings
{
    public string AccessToken { get; set; }
    public string NotificationUrl { get; set; }
    public int DefaultExpirationMinutes { get; set; }
}

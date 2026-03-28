using MercadoPago.Config;
using Payment.Infra.Configurations;

namespace Payment.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddMercadoPagoToken(this IServiceCollection services, IConfiguration configuration)
    {
        var mercadoPagoSettings = configuration.GetSection("MercadoPago").Get<MercadoPagoSettings>();
        MercadoPagoConfig.AccessToken = mercadoPagoSettings!.AccessToken;

        return services;
    }
}

using FluentResults;
using MediatR;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Error;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Payment.Application.Commands.Handlers;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Preference>>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public CreatePaymentCommandHandler(IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = Log.ForContext<CreatePaymentCommandHandler>();
    }

    public async Task<Result<Preference>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

        if (string.IsNullOrEmpty(MercadoPagoConfig.AccessToken))
        {
            _logger.Error("[ERROR] MercadoPago Access Token is not configured.");
            return Result.Fail("MercadoPago Access Token is not configured.");
        }

        _logger.Information("[INFO] Creating payment preference for {EventId}", request.EventId);

        try
        {
            var paymentItems = new List<PreferenceItemRequest>();

            foreach (var item in request.Items)
            {
                paymentItems.Add(new PreferenceItemRequest
                {
                    Title = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            var paymentRequest = new PreferenceRequest
            { 
                Items = paymentItems,
                NotificationUrl = _configuration["MercadoPago:NotificationUrl"],
                ExternalReference = request.EventId.ToString()
            };

            _logger.Information("[INFO] Preference request created with {ItemCount} items", paymentItems.Count);

            var client = new PreferenceClient();
            var preference = await client.CreateAsync(paymentRequest);

            _logger.Information("[INFO] Payment preference created with ID: {PreferenceId}", preference.Id);
            Console.WriteLine(preference.InitPoint);

            return Result.Ok(preference);
        }
        catch (MercadoPagoApiException ex)
        {
            _logger.Error(
                ex,
                "MercadoPago API error. StatusCode: {StatusCode}, Message: {Message}",
                ex.StatusCode, ex.Message
            );

            return Result.Fail(ex.Message);
        }
    }
}

using FluentResults;
using MediatR;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;

namespace Payment.Application.Commands.Handlers;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<Preference>>
{
    public async Task<Result<Preference>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        MercadoPagoConfig.AccessToken = "APP_USR-6405780802792810-121921-f7aa25cf72f34e7634cf16edfc702f15-3081056684";

        var paymentRequest = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Quantity = request.Quantity,
                        UnitPrice = request.UnitPrice
                    }
                }
        };

        var client = new PreferenceClient();
        var preference = await client.CreateAsync(paymentRequest);

        return Result.Ok(preference);
    }
}

using FluentResults;
using MediatR;
using MercadoPago.Client.Preference;

namespace Payment.Application.Commands.Handlers;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
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

        return Result.Ok(1);
    }
}

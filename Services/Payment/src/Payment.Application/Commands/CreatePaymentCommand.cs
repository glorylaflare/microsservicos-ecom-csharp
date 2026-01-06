using FluentResults;
using MediatR;
using MercadoPago.Resource.Preference;

namespace Payment.Application.Commands;

public record class CreatePaymentCommand(
    string Title, 
    string Description, 
    int Quantity, 
    decimal UnitPrice
) : IRequest<Result<Preference>>;

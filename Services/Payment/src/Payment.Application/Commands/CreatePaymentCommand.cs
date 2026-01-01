using FluentResults;
using MediatR;

namespace Payment.Application.Commands;

public record class CreatePaymentCommand(
    string Title, 
    string Description, 
    int Quantity, 
    decimal UnitPrice
) : IRequest<Result<int>>;

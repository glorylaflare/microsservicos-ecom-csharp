using FluentResults;
using MediatR;
using System.Text.Json.Serialization;

namespace Payment.Application.Commands;

public class ProcessPaymentCommand : IRequest<Result<Unit>>
{
    public required string Type { get; set; }
    public required PaymentData Data { get; set; }
}

public class PaymentData
{
    public required string Id { get; set; }
}
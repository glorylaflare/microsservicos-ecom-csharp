using BuildingBlocks.Contracts;
using FluentResults;
using MediatR;
using MercadoPago.Resource.Preference;

namespace Payment.Application.Commands;

public record class CreatePaymentCommand(Guid EventId, List<ProductItemDto> Items) : IRequest<Result<Preference>>;

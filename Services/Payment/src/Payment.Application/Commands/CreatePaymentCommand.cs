using BuildingBlocks.Contracts;
using FluentResults;
using MediatR;
using MercadoPago.Resource.Preference;

namespace Payment.Application.Commands;

public record CreatePaymentCommand(Guid EventId, List<ProductItemDto> Items) : IRequest<Result<Preference>>;

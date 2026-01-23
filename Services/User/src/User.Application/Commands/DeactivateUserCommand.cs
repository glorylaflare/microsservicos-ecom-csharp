using FluentResults;
using MediatR;
namespace User.Application.Commands;

public record DeactivateUserCommand(string Email) : IRequest<Result>;
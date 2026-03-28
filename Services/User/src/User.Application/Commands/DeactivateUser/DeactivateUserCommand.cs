using FluentResults;
using MediatR;

namespace User.Application.Commands.DeactivateUser;

public record DeactivateUserCommand(string Email) : IRequest<Result>;
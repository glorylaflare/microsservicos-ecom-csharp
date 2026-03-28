using FluentResults;
using MediatR;

namespace User.Application.Commands.CreateUser;

public record CreateUserCommand(
    string Username,
    string Email,
    string Password
) : IRequest<Result<int>>;
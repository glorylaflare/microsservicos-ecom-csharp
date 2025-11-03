using FluentResults;
using MediatR;

namespace User.Application.Commands;

public record CreateUserCommand(
    string Username, 
    string Email,
    string Password
) : IRequest<Result<int>>;

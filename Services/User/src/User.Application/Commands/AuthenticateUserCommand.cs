using FluentResults;
using MediatR;
using User.Application.Responses;

namespace User.Application.Commands;

public record AuthenticateUserCommand(
    string Email, 
    string Password
) : IRequest<Result<TokenResponse>>;
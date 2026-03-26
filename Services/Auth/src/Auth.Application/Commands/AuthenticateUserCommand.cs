using Auth.Application.Responses;
using FluentResults;
using MediatR;

namespace Auth.Application.Commands;

public record AuthenticateUserCommand(
    string Email,
    string Password
) : IRequest<Result<TokenResponse>>;
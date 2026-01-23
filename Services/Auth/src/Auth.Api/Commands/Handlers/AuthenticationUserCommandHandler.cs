using Auth.Api.Interfaces;
using Auth.Api.Responses;
using Auth0.Core.Exceptions;
using FluentResults;
using FluentValidation;
using MediatR;
using Serilog;
namespace Auth.Api.Commands.Handlers;

public class AuthenticationUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<TokenResponse>>
{
    private readonly IValidator<AuthenticateUserCommand> _validator;
    private readonly IAuthService _authService;
    private readonly Serilog.ILogger _logger;
    public AuthenticationUserCommandHandler(IValidator<AuthenticateUserCommand> validator, IAuthService authService)
    {
        _validator = validator;
        _authService = authService;
        _logger = Log.ForContext<AuthenticationUserCommandHandler>();
    }
    public async Task<Result<TokenResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("[INFO] Handling {EventName} for email: {Email}", nameof(AuthenticateUserCommand), request.Email);
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));
            _logger.Warning("[WARN] Validation failed for {EventName}: {Errors}", nameof(AuthenticateUserCommand), errors);
            return Result.Fail(errors);
        }
        try
        {
            var tokenResponse = await _authService.GetTokenAsync(request.Email, request.Password);
            var response = new TokenResponse(
                tokenResponse.AccessToken,
                tokenResponse.IdToken,
                tokenResponse.ExpiresIn
            );
            _logger.Information("[INFO] User authenticated successfully with email: {Email}", request.Email);
            return Result.Ok(response);
        }
        catch (ApiException ex)
        {
            _logger.Warning(ex, "[WARN] Authentication failed for user with email: {Email} - {Message}", request.Email, ex.Message);
            return Result.Fail("Invalid email or password.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "[ERROR] Error occurred while authenticating user with email: {Email}", request.Email);
            return Result.Fail("An error occurred while authenticating the user.");
        }
    }
}
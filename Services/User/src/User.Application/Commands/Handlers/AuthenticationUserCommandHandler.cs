using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;
using User.Application.Responses;

namespace User.Application.Commands.Handlers;

public class AuthenticationUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result<TokenResponse>>
{
    private readonly IValidator<AuthenticateUserCommand> _validator;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public AuthenticationUserCommandHandler(IValidator<AuthenticateUserCommand> validator, IConfiguration configuration)
    {
        _validator = validator;
        _configuration = configuration;
        _logger = Log.ForContext<AuthenticationUserCommandHandler>();
    }

    public async Task<Result<TokenResponse>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Handling {EventName} for email: {Email}", nameof(AuthenticateUserCommand), request.Email);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => new Error(e.ErrorMessage));

            _logger.Warning("Validation failed for AuthenticateUserCommand: {Errors}", errors);
            return Result.Fail(errors);
        }

        try
        {
            _logger.Information("Authenticating user with email: {Email}", request.Email);    
            var domain = _configuration["Auth0:Domain"];
            var clientId = _configuration["Auth0:ClientId"]!;
            var clientSecret = _configuration["Auth0:ClientSecret"]!;
            var audience = _configuration["Auth0:Audience"]!;

            var authClient = new AuthenticationApiClient(
                new Uri($"https://{domain}"));

            var tokenRequest = new ResourceOwnerTokenRequest
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = "openid profile email",
                Audience = audience,
                Username = request.Email,
                Password = request.Password,
                Realm = "Username-Password-Authentication"
            };

            var tokenResponse = await authClient.GetTokenAsync(tokenRequest);
            
            var response = new TokenResponse(
                tokenResponse.AccessToken,
                tokenResponse.IdToken,
                tokenResponse.ExpiresIn
            );

            _logger.Information("User authenticated successfully with email: {Email}", request.Email);
            return Result.Ok(response);
        }
        catch (ApiException ex)
        {
            _logger.Warning(ex, "Authentication failed for user with email: {Email} - {Message}", request.Email, ex.Message);
            return Result.Fail("Invalid email or password.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while authenticating user with email: {Email}", request.Email);
            return Result.Fail("An error occurred while authenticating the user.");
        }
    }
}

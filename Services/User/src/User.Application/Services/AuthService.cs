using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using User.Application.Interfaces;

namespace User.Application.Services;

public class AuthService : IAuthService
{
    private readonly AuthenticationApiClient _authClient;
    private readonly ILogger _logger;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _audience;

    public AuthService(IConfiguration config)
    {
        var domain = config["Auth0:Domain"]!;
        _clientId = config["Auth0:ClientId"]!;
        _clientSecret = config["Auth0:ClientSecret"]!;
        _audience = config["Auth0:Audience"]!;
        _authClient = new AuthenticationApiClient(new Uri($"https://{domain}/"));
        _logger = Log.ForContext<AuthService>();
    }


    public async Task<SignupUserResponse> SignupUserAsync(string email, string password)
    {
        _logger.Information("Signing up user with email: {Email}", email);

        return await _authClient.SignupUserAsync(new SignupUserRequest
        {
            ClientId = _clientId,
            Email = email,
            Password = password,
            Connection = "Username-Password-Authentication"
        });
    }

    public async Task<AccessTokenResponse> GetTokenAsync(string email, string password)
    {
        _logger.Information("Authenticating user with email: {Email}", email);

        return await _authClient.GetTokenAsync(new ResourceOwnerTokenRequest
        {
            ClientId = _clientId,
            ClientSecret = _clientSecret,
            Audience = _audience,
            Scope = "openid profile email",
            Username = email,
            Password = password,
            Realm = "Username-Password-Authentication"
        });
    }
}

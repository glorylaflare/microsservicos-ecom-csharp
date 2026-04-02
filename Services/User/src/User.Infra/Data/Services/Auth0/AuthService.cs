using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Options;
using Serilog;
using User.Application.Interfaces;
using User.Domain.Models;

namespace User.Infra.Data.Services.Auth0;

public class AuthService : IAuthService
{
    private readonly AuthenticationApiClient _authClient;
    private readonly ILogger _logger;
    private readonly Auth0Settings _auth0Settings;

    public AuthService(IOptions<Auth0Settings> auth0Settings)
    {
        _auth0Settings = auth0Settings.Value;
        _authClient = new AuthenticationApiClient(new Uri($"https://{_auth0Settings.Domain}/"));
        _logger = Log.ForContext<AuthService>();
    }

    public async Task<SignupUserResponse> SignupUserAsync(string email, string password)
    {
        _logger.Information("[INFO] Signing up new user");

        return await _authClient.SignupUserAsync(new SignupUserRequest
        {
            ClientId = _auth0Settings.ClientId,
            Email = email,
            Password = password,
            Connection = "Username-Password-Authentication"
        });
    }
}
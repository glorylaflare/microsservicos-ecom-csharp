using Auth.Application.Interfaces;
using Auth.Infra.Configurations;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Microsoft.Extensions.Options;
using Serilog;

namespace Auth.Infra.Services
{
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

        public async Task<AccessTokenResponse> GetTokenAsync(string email, string password)
        {
            _logger.Information("[INFO] Authenticating user with email: {Email}", email);

            return await _authClient.GetTokenAsync(new ResourceOwnerTokenRequest
            {
                ClientId = _auth0Settings.ClientId,
                ClientSecret = _auth0Settings.ClientSecret,
                Audience = _auth0Settings.Audience,
                Scope = "openid profile email",
                Username = email,
                Password = password,
                Realm = "Username-Password-Authentication"
            });
        }
    }
}
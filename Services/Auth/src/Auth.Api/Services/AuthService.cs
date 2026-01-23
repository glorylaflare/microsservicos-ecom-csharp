using Auth.Api.Interfaces;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Serilog;
namespace Auth.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthenticationApiClient _authClient;
        private readonly Serilog.ILogger _logger;
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
        public async Task<AccessTokenResponse> GetTokenAsync(string email, string password)
        {
            _logger.Information("[INFO] Authenticating user with email: {Email}", email);
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
}
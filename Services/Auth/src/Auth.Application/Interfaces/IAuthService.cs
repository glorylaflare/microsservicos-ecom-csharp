using Auth0.AuthenticationApi.Models;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<AccessTokenResponse> GetTokenAsync(string email, string password);
}
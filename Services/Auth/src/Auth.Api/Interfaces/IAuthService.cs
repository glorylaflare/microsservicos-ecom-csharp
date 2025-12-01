using Auth0.AuthenticationApi.Models;

namespace Auth.Api.Interfaces
{
    public interface IAuthService
    {
        Task<AccessTokenResponse> GetTokenAsync(string email, string password);
    }
}

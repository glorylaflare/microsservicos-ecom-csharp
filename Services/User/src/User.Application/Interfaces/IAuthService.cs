using Auth0.AuthenticationApi.Models;

namespace User.Application.Interfaces;

public interface IAuthService
{
    Task<SignupUserResponse> SignupUserAsync(string email, string password);
    Task<AccessTokenResponse> GetTokenAsync(string email, string password);
}

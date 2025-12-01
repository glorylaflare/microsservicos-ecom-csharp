namespace Auth.Api.Responses;

public record TokenResponse(
    string AccessToken,
    string IdToken,
    int ExpiresIn
);
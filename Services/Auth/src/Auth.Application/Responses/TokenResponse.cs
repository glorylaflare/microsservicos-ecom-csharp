namespace Auth.Application.Responses;

public record TokenResponse(
    string AccessToken,
    string IdToken,
    int ExpiresIn
);
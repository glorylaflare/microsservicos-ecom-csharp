namespace User.Application.Responses;

public record UserResponse(
    int Id,
    string Username,
    string Email,
    int Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
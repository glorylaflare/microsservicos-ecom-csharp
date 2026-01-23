namespace User.Application.Responses;

public record GetUserResponse(
    int Id,
    string Username,
    string Email,
    int Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
namespace User.Application.Responses;

public record GetUserResponse(
    int Id, 
    string Username, 
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
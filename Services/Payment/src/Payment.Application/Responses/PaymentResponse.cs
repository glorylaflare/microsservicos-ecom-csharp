namespace Payment.Application.Responses;

public record PaymentResponse(int Id, string Status, DateTime CreatedAt);
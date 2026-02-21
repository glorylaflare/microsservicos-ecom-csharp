namespace Payment.Domain.Interface;

public interface IPaymentRepository
{
    Task<Models.Payment?> GetByIdAsync(int orderId);
    Task AddAsync(Models.Payment payment);
    void Update(Models.Payment payment);
    Task SaveChangesAsync();
    Task<IReadOnlyList<Models.Payment>> GetExpiredPaymentsAsync(DateTime currentTime);
    Task<int> SetExpiredPaymentsAsync(DateTime currentTime);
}
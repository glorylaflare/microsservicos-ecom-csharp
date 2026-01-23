namespace Payment.Domain.Interface;

public interface IPaymentRepository
{
    Task<Models.Payment?> GetByIdAsync(int orderId);
    Task AddAsync(Models.Payment payment);
    void Update(Models.Payment payment);
    Task SaveChangesAsync();
}
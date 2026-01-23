using Microsoft.EntityFrameworkCore;
using Payment.Domain.Interface;
using Payment.Infra.Data.Context.Write;
namespace Payment.Infra.Data.Repositories;
public class PaymentRepository : IPaymentRepository
{
    private readonly DbSet<Domain.Models.Payment> _payments;
    private readonly WriteDbContext _context;
    public PaymentRepository(WriteDbContext context)
    {
        _payments = context.Payments;
        _context = context;
    }
    public async Task AddAsync(Domain.Models.Payment payment) => await _payments.AddAsync(payment);
    public async Task<Domain.Models.Payment?> GetByIdAsync(int orderId) => await _payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    public void Update(Domain.Models.Payment payment) => _payments.Update(payment);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
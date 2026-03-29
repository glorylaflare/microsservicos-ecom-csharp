using BuildingBlocks.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Interface;
using Payment.Domain.Models;
using Payment.Infra.Data.Context.Write;

namespace Payment.Infra.Data.Repositories;

public class PaymentRepository : Repository<Domain.Models.Payment>, IPaymentRepository
{
    public PaymentRepository(WriteDbContext context) : base(context) { }

    public async Task<int> SetExpiredPaymentsAsync(DateTime currentTime) => await _dbSet
            .Where(p => p.Status == PaymentStatus.Pending && p.ExpirationDate <= currentTime)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, PaymentStatus.Failed)
                                      .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
using BuildingBlocks.Infra.ReadModels;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Interfaces;
using Payment.Infra.Data.Context.Read;

namespace Payment.Infra.Data.Services;

public class PaymentReadService : IPaymentReadService
{
    public readonly DbSet<PaymentReadModel> _payments;

    public PaymentReadService(ReadDbContext context)
    {
        _payments = context.Payments;
    }

    public async Task<IEnumerable<PaymentReadModel>> GetAllAsync()
    {
        return await _payments.Select(p => new PaymentReadModel
        {
            Id = p.Id,
            OrderId = p.OrderId,
            Amount = p.Amount,
            Status = p.Status,
            CheckoutUrl = p.CheckoutUrl,
            CreatedDate = p.CreatedDate,
            UpdatedAt = p.UpdatedAt
        })
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<PaymentReadModel?> GetByIdAsync(int paymentId)
    {
        return await _payments.Where(p => p.Id == paymentId)
            .Select(p => new PaymentReadModel
            {
                Id = p.Id,
                OrderId = p.OrderId,
                Amount = p.Amount,
                Status = p.Status,
                CheckoutUrl = p.CheckoutUrl,
                CreatedDate = p.CreatedDate,
                UpdatedAt = p.UpdatedAt
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using System.Data;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class PaymentService : IPaymentService
{
    private readonly ZBooksDbContext _context;

    public PaymentService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentDto>> GetPaymentsAsync(int userId)
    {
        var payments = await _context.Payments
            .AsNoTracking()
            .Where(p => p.Purchase.UserId == userId)
            .ToListAsync();

        return payments.Select(ToDto).ToList();
    }

    public async Task<PaymentDto?> GetPaymentAsync(int id, int userId)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PaymentId == id && p.Purchase.UserId == userId);

        return payment == null ? null : ToDto(payment);
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, int userId)
    {
        var payments = await CreatePaymentsAsync(new CreateBatchPaymentDto
        {
            PurchaseIds = [dto.PurchaseId],
            PaymentReference = dto.PaymentReference,
            PaymentMethod = dto.PaymentMethod
        }, userId);
        return payments[0];
    }

    public async Task<List<PaymentDto>> CreatePaymentsAsync(CreateBatchPaymentDto dto, int userId)
    {
        if (dto.PurchaseIds is null)
            throw new InvalidOperationException("Select at least one valid purchase to pay for.");
        var purchaseIds = dto.PurchaseIds.Distinct().ToList();
        if (purchaseIds.Count == 0 || purchaseIds.Count != dto.PurchaseIds.Count || purchaseIds.Any(id => id <= 0))
            throw new InvalidOperationException("Select at least one valid purchase to pay for.");
        if (string.IsNullOrWhiteSpace(dto.PaymentReference))
            throw new InvalidOperationException("A payment reference is required.");

        var reference = dto.PaymentReference.Trim();
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        if (await _context.Payments.AnyAsync(payment => payment.PaymentReference == reference))
            throw new InvalidOperationException("This payment reference has already been used.");

        var purchases = await _context.Purchases
            .Where(purchase => purchaseIds.Contains(purchase.PurchaseId) && purchase.UserId == userId)
            .ToListAsync();

        if (purchases.Count != purchaseIds.Count)
            throw new InvalidOperationException("One or more purchases could not be found for the current user.");
        if (purchases.Any(purchase => purchase.Status != "Pending"))
            throw new InvalidOperationException("Only pending purchases can be paid.");

        var paidAt = DateTime.UtcNow;
        var entities = purchases.Select(purchase =>
        {
            purchase.Status = "Completed";
            return new Payment
            {
                PurchaseId = purchase.PurchaseId,
                Amount = purchase.Amount,
                PaymentReference = reference,
                PaymentMethod = dto.PaymentMethod,
                Status = "Completed",
                PaidAt = paidAt
            };
        }).ToList();

        _context.Payments.AddRange(entities);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return entities.Select(ToDto).ToList();
    }

    public async Task<PaymentDto?> UpdatePaymentAsync(int id, UpdatePaymentDto dto)
    {
        var entity = await _context.Payments.FindAsync(id);
        if (entity == null) return null;

        entity.PaymentMethod = dto.PaymentMethod;
        entity.Status = dto.Status;
        entity.PaidAt = dto.PaidAt;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeletePaymentAsync(int id)
    {
        var entity = await _context.Payments.FindAsync(id);
        if (entity == null) return false;

        _context.Payments.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static PaymentDto ToDto(Payment p) => new()
    {
        PaymentId = p.PaymentId,
        PurchaseId = p.PurchaseId,
        Amount = p.Amount,
        PaymentReference = p.PaymentReference,
        PaymentMethod = p.PaymentMethod,
        Status = p.Status,
        PaidAt = p.PaidAt
    };
}

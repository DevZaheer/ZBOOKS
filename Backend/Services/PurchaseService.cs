using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class PurchaseService : IPurchaseService
{
    private readonly ZBooksDbContext _context;

    public PurchaseService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseDto>> GetPurchasesAsync(int userId)
    {
        var purchases = await _context.Purchases
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToListAsync();

        return purchases.Select(ToDto).ToList();
    }

    public async Task<PurchaseDto?> GetPurchaseAsync(int id, int userId)
    {
        var purchase = await _context.Purchases
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PurchaseId == id && p.UserId == userId);

        return purchase == null ? null : ToDto(purchase);
    }

    public async Task<PurchaseDto> CreatePurchaseAsync(CreatePurchaseDto dto, int userId)
    {
        var book = await _context.Books.FindAsync(dto.BookId);

        if (book == null)
            throw new InvalidOperationException("Book not found.");

        // Duplicate protection: only Pending and Completed purchases block a
        // new one, so a user can retry after a Failed or Cancelled purchase.
        var existingStatuses = await _context.Purchases
            .Where(p => p.UserId == userId
                     && p.BookId == dto.BookId
                     && (p.Status == "Pending" || p.Status == "Completed"))
            .Select(p => p.Status)
            .Distinct()
            .ToListAsync();

        if (existingStatuses.Contains("Completed"))
            throw new InvalidOperationException("You already own this book.");

        if (existingStatuses.Contains("Pending"))
            throw new InvalidOperationException(
                "You already have a pending purchase for this book. Complete its payment instead of creating a new purchase.");

        if (!book.IsAvailable)
            throw new InvalidOperationException("This book is not currently available for purchase.");

        var entity = new Purchase
        {
            UserId = userId,
            BookId = dto.BookId,
            Amount = book.Price,
            Status = "Pending",
            PurchasedAt = DateTime.UtcNow
        };

        _context.Purchases.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<List<PurchaseDto>> CreatePurchasesAsync(CreatePurchasesDto dto, int userId)
    {
        if (dto.BookIds is null)
            throw new InvalidOperationException("Select at least one valid book to purchase.");
        var bookIds = dto.BookIds.Distinct().ToList();
        if (bookIds.Count == 0 || bookIds.Count != dto.BookIds.Count || bookIds.Any(id => id <= 0))
            throw new InvalidOperationException("Select at least one valid book to purchase.");

        var books = await _context.Books
            .Where(book => bookIds.Contains(book.BookId))
            .ToListAsync();

        if (books.Count != bookIds.Count)
            throw new InvalidOperationException("One or more selected books could not be found.");

        var existingPurchases = await _context.Purchases
            .Where(purchase => purchase.UserId == userId
                && bookIds.Contains(purchase.BookId)
                && (purchase.Status == "Pending" || purchase.Status == "Completed"))
            .Select(purchase => new { purchase.BookId, purchase.Status })
            .ToListAsync();

        foreach (var bookId in bookIds)
        {
            var status = existingPurchases.FirstOrDefault(purchase => purchase.BookId == bookId)?.Status;
            if (status == "Completed")
                throw new InvalidOperationException("You already own one or more selected books.");
            if (status == "Pending")
                throw new InvalidOperationException("You already have a pending purchase for one or more selected books.");
        }

        if (books.Any(book => !book.IsAvailable))
            throw new InvalidOperationException("One or more selected books are not currently available for purchase.");

        var purchasedAt = DateTime.UtcNow;
        var entities = books.Select(book => new Purchase
        {
            UserId = userId,
            BookId = book.BookId,
            Amount = book.Price,
            Status = "Pending",
            PurchasedAt = purchasedAt
        }).ToList();

        _context.Purchases.AddRange(entities);
        await _context.SaveChangesAsync();
        return entities.Select(ToDto).ToList();
    }

    public async Task<PurchaseDto?> UpdatePurchaseAsync(int id, UpdatePurchaseDto dto)
    {
        var entity = await _context.Purchases.FindAsync(id);
        if (entity == null) return null;

        entity.Status = dto.Status;
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeletePurchaseAsync(int id)
    {
        var entity = await _context.Purchases.FindAsync(id);
        if (entity == null) return false;

        _context.Purchases.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> HasPurchasedBookAsync(int userId, int bookId) =>
        _context.Purchases.AnyAsync(p =>
            p.UserId == userId && p.BookId == bookId && p.Status == "Completed");

    private static PurchaseDto ToDto(Purchase p) => new()
    {
        PurchaseId = p.PurchaseId,
        UserId = p.UserId,
        BookId = p.BookId,
        Amount = p.Amount,
        PurchasedAt = p.PurchasedAt,
        Status = p.Status
    };
}

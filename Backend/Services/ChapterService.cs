using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class ChapterService : IChapterService
{
    private readonly ZBooksDbContext _context;
    private readonly IPurchaseService _purchaseService;

    public ChapterService(ZBooksDbContext context, IPurchaseService purchaseService)
    {
        _context = context;
        _purchaseService = purchaseService;
    }

    public async Task<List<ChapterDto>> GetChaptersAsync(int userId, bool isAdmin, int? bookId)
    {
        var query = _context.Chapters.AsQueryable();

        // Previously this always loaded every chapter in the whole system
        // and filtered in memory. Now it's filtered in the query, and callers
        // (e.g. a "read this book" screen in React) can scope to one book.
        if (bookId.HasValue)
            query = query.Where(c => c.BookId == bookId.Value);

        if (!isAdmin)
        {
            var ownedBookIds = _context.Purchases
                .Where(p => p.UserId == userId && p.Status == "Completed")
                .Select(p => p.BookId);

            query = query.Where(c => ownedBookIds.Contains(c.BookId));
        }

        var chapters = await query
            .OrderBy(c => c.BookId)
            .ThenBy(c => c.ChapterNumber)
            .ToListAsync();

        return chapters.Select(ToDto).ToList();
    }

    public async Task<ChapterDto?> GetChapterAsync(int id, int userId, bool isAdmin)
    {
        var entity = await _context.Chapters.FirstOrDefaultAsync(c => c.ChapterId == id);
        if (entity == null) return null;

        if (!isAdmin && !await _purchaseService.HasPurchasedBookAsync(userId, entity.BookId))
            throw new UnauthorizedAccessException("You must purchase this book to read this chapter.");

        return ToDto(entity);
    }

    public async Task<ChapterDto> CreateChapterAsync(CreateChapterDto dto)
    {
        var bookExists = await _context.Books.AnyAsync(b => b.BookId == dto.BookId);
        if (!bookExists)
            throw new InvalidOperationException("Book not found.");

        var entity = new Chapter
        {
            BookId = dto.BookId,
            Title = dto.Title,
            ChapterNumber = dto.ChapterNumber,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Chapters.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<ChapterDto?> UpdateChapterAsync(int id, UpdateChapterDto dto)
    {
        var entity = await _context.Chapters.FindAsync(id);
        if (entity == null) return null;

        entity.Title = dto.Title;
        entity.ChapterNumber = dto.ChapterNumber;
        entity.Content = dto.Content;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteChapterAsync(int id)
    {
        var entity = await _context.Chapters.FindAsync(id);
        if (entity == null) return false;

        _context.Chapters.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ChapterDto ToDto(Chapter e) => new()
    {
        ChapterId = e.ChapterId,
        BookId = e.BookId,
        Title = e.Title,
        ChapterNumber = e.ChapterNumber,
        Content = e.Content,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}

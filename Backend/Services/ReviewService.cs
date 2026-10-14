using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class ReviewService : IReviewService
{
    private readonly ZBooksDbContext _context;
    private readonly IPurchaseService _purchaseService;

    public ReviewService(ZBooksDbContext context, IPurchaseService purchaseService)
    {
        _context = context;
        _purchaseService = purchaseService;
    }

    public async Task<List<ReviewDto>> GetReviewsAsync()
    {
        var reviews = await _context.Reviews.AsNoTracking().ToListAsync();
        return reviews.Select(ToDto).ToList();
    }

    public async Task<ReviewDto?> GetReviewAsync(int id)
    {
        var review = await _context.Reviews.AsNoTracking()
            .FirstOrDefaultAsync(r => r.ReviewId == id);
        return review == null ? null : ToDto(review);
    }

    public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int userId)
    {
        if (!await _purchaseService.HasPurchasedBookAsync(userId, dto.BookId))
            throw new InvalidOperationException("You must purchase this book before reviewing it.");

        if (await _context.Reviews.AnyAsync(r => r.UserId == userId && r.BookId == dto.BookId))
            throw new InvalidOperationException("You have already reviewed this book.");

        var entity = new Review
        {
            UserId = userId,
            BookId = dto.BookId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<ReviewDto?> UpdateReviewAsync(int id, UpdateReviewDto dto, int userId)
    {
        var entity = await _context.Reviews
            .FirstOrDefaultAsync(r => r.ReviewId == id && r.UserId == userId);

        if (entity == null) return null;

        entity.Rating = dto.Rating;
        entity.Comment = dto.Comment;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteReviewAsync(int id, int userId, bool isAdmin)
    {
        var entity = await _context.Reviews
            .FirstOrDefaultAsync(r => r.ReviewId == id && (isAdmin || r.UserId == userId));

        if (entity == null) return false;

        _context.Reviews.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ReviewDto ToDto(Review r) => new()
    {
        ReviewId = r.ReviewId,
        UserId = r.UserId,
        BookId = r.BookId,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
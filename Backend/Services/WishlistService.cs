using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class WishlistService : IWishlistService
{
    private readonly ZBooksDbContext _context;

    public WishlistService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<List<WishlistDto>> GetWishlistsAsync(int userId)
    {
        return await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistDto
            {
                WishlistId = w.WishlistId,
                UserId = w.UserId,
                BookId = w.BookId,
                AddedAt = w.AddedAt
            })
            .ToListAsync();
    }

    public async Task<WishlistDto?> GetWishlistAsync(
        int id,
        int userId)
    {
        return await _context.Wishlists
            .Where(w =>
                w.WishlistId == id &&
                w.UserId == userId)
            .Select(w => new WishlistDto
            {
                WishlistId = w.WishlistId,
                UserId = w.UserId,
                BookId = w.BookId,
                AddedAt = w.AddedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<WishlistDto> CreateWishlistAsync(
        CreateWishlistDto dto,
        int userId)
    {
        var entity = new Wishlist
        {
            UserId = userId,
            BookId = dto.BookId,
            AddedAt = DateTime.UtcNow
        };

        _context.Wishlists.Add(entity);

        await _context.SaveChangesAsync();

        return new WishlistDto
        {
            WishlistId = entity.WishlistId,
            UserId = entity.UserId,
            BookId = entity.BookId,
            AddedAt = entity.AddedAt
        };
    }

    public async Task<bool> DeleteWishlistAsync(
        int id,
        int userId)
    {
        var entity = await _context.Wishlists
            .FirstOrDefaultAsync(w =>
                w.WishlistId == id &&
                w.UserId == userId);

        if (entity == null)
            return false;

        _context.Wishlists.Remove(entity);

        await _context.SaveChangesAsync();

        return true;
    }
}
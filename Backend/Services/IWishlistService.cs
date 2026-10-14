using ZBooksApi.DTOs;

namespace ZBooksApi.Services;

public interface IWishlistService
{
    Task<List<WishlistDto>> GetWishlistsAsync(int userId);
    Task<WishlistDto?> GetWishlistAsync(int id, int userId);
    Task<WishlistDto> CreateWishlistAsync(CreateWishlistDto dto, int userId);
    Task<bool> DeleteWishlistAsync(int id, int userId);
}
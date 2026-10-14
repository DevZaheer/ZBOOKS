using ZBooksApi.DTOs;

namespace ZBooksApi.Services;

public interface IPurchaseService
{
    Task<bool> HasPurchasedBookAsync(int userId, int bookId);
    Task<List<PurchaseDto>> GetPurchasesAsync(int userId);
    Task<PurchaseDto?> GetPurchaseAsync(int id, int userId);
    Task<PurchaseDto> CreatePurchaseAsync(CreatePurchaseDto dto, int userId);
    Task<List<PurchaseDto>> CreatePurchasesAsync(CreatePurchasesDto dto, int userId);
    Task<PurchaseDto?> UpdatePurchaseAsync(int id, UpdatePurchaseDto dto);
    Task<bool> DeletePurchaseAsync(int id);
}

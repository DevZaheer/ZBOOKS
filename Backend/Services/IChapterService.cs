using ZBooksApi.DTOs;

namespace ZBooksApi.Services;

public interface IChapterService
{
    Task<List<ChapterDto>> GetChaptersAsync(int userId, bool isAdmin, int? bookId);
    Task<ChapterDto?> GetChapterAsync(int id, int userId, bool isAdmin);
    Task<ChapterDto> CreateChapterAsync(CreateChapterDto dto);
    Task<ChapterDto?> UpdateChapterAsync(int id, UpdateChapterDto dto);
    Task<bool> DeleteChapterAsync(int id);
}

using ZBooksApi.DTOs;

namespace ZBooksApi.Services;

public interface IReadingProgressService
{
    Task<List<ReadingProgressDto>> GetReadingProgressesAsync(int userId);
    Task<ReadingProgressDto?> GetReadingProgressAsync(int id, int userId);
    Task<ReadingProgressDto> CreateReadingProgressAsync(CreateReadingProgressDto dto, int userId);
    Task<ReadingProgressDto?> UpdateReadingProgressAsync(int id, UpdateReadingProgressDto dto, int userId);
    Task<bool> DeleteReadingProgressAsync(int id, int userId);
}

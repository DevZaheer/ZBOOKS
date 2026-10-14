using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class ReadingProgressService : IReadingProgressService
{
    private readonly ZBooksDbContext _context;

    public ReadingProgressService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReadingProgressDto>> GetReadingProgressesAsync(
        int userId)
    {
        var progress = await _context.ReadingProgresses
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return progress.Select(ToDto).ToList();
    }

    public async Task<ReadingProgressDto?> GetReadingProgressAsync(
        int id,
        int userId)
    {
        var progress = await _context.ReadingProgresses
            .Where(r =>
                r.ReadingProgressId == id &&
                r.UserId == userId)
            .FirstOrDefaultAsync();

        return progress == null ? null : ToDto(progress);
    }

    public async Task<ReadingProgressDto> CreateReadingProgressAsync(
        CreateReadingProgressDto dto,
        int userId)
    {
        var completionPercent = await CalculateCompletionPercentAsync(
            dto.BookId,
            dto.CurrentChapter);

        var entity = new ReadingProgress
        {
            UserId = userId,
            BookId = dto.BookId,
            CurrentChapter = dto.CurrentChapter,
            CurrentPage = dto.CurrentPage,
            ProgressPercentage = completionPercent,
            LastReadAt = DateTime.UtcNow
        };

        _context.ReadingProgresses.Add(entity);

        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<ReadingProgressDto?> UpdateReadingProgressAsync(
        int id,
        UpdateReadingProgressDto dto,
        int userId)
    {
        var entity = await _context.ReadingProgresses
            .FirstOrDefaultAsync(r =>
                r.ReadingProgressId == id &&
                r.UserId == userId);

        if (entity == null)
            return null;

        var completionPercent = await CalculateCompletionPercentAsync(
            entity.BookId,
            dto.CurrentChapter);

        entity.CurrentChapter = dto.CurrentChapter;
        entity.CurrentPage = dto.CurrentPage;
        entity.ProgressPercentage = completionPercent;
        entity.LastReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task<bool> DeleteReadingProgressAsync(
        int id,
        int userId)
    {
        var entity = await _context.ReadingProgresses
            .FirstOrDefaultAsync(r =>
                r.ReadingProgressId == id &&
                r.UserId == userId);

        if (entity == null)
            return false;

        _context.ReadingProgresses.Remove(entity);

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<decimal> CalculateCompletionPercentAsync(
        int bookId,
        int chapter)
    {
        var totalChapters = await _context.Chapters
            .CountAsync(c => c.BookId == bookId);

        if (chapter < 0 || chapter > totalChapters)
            throw new ArgumentException(
                $"Chapter must be between 0 and {totalChapters}.",
                nameof(chapter));

        return totalChapters == 0
            ? 0
            : Math.Round(chapter * 100m / totalChapters, 1);
    }

    private static ReadingProgressDto ToDto(ReadingProgress progress)
    {
        var status = progress.ProgressPercentage == 0
            ? "Not started"
            : progress.ProgressPercentage >= 100
                ? "Completed"
                : "Reading";

        return new ReadingProgressDto
        {
            ReadingProgressId = progress.ReadingProgressId,
            UserId = progress.UserId,
            BookId = progress.BookId,
            CurrentChapter = progress.CurrentChapter,
            CurrentPage = progress.CurrentPage,
            CompletionPercent = progress.ProgressPercentage,
            Status = status,
            LastReadAt = progress.LastReadAt
        };
    }
}
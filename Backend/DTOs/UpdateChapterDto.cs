namespace ZBooksApi.DTOs;

public class UpdateChapterDto
{
    public string Title { get; set; } = null!;
    public int ChapterNumber { get; set; }
    public string? Content { get; set; }
}

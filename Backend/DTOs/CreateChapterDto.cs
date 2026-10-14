namespace ZBooksApi.DTOs;

public class CreateChapterDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public int ChapterNumber { get; set; }
    public string? Content { get; set; }
}

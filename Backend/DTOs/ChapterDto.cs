namespace ZBooksApi.DTOs;

public class ChapterDto
{
    public int ChapterId { get; set; }
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public int ChapterNumber { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

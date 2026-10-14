namespace ZBooksApi.DTOs;

public class ReadingProgressDto
{
    public int ReadingProgressId { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public int CurrentChapter { get; set; }
    public int CurrentPage { get; set; }
    public decimal CompletionPercent { get; set; }
    public string Status { get; set; } = null!;
    public DateTime LastReadAt { get; set; }
}

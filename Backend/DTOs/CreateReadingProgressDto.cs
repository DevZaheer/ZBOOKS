namespace ZBooksApi.DTOs;

public class CreateReadingProgressDto
{
    public int BookId { get; set; }
    public int CurrentChapter { get; set; }
    public int CurrentPage { get; set; }
}
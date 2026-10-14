namespace ZBooksApi.DTOs;

public class CreateReviewDto
{
    public int BookId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
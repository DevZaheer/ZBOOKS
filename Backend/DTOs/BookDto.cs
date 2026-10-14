namespace ZBooksApi.DTOs;

public class BookDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public decimal Price { get; set; }
    public DateOnly PublicationDate { get; set; }
    public bool IsAvailable { get; set; }
    public List<int> AuthorIds { get; set; } = new();
    public List<int> CategoryIds { get; set; } = new();

    // 0 when the book has no reviews yet; check ReviewCount to tell "unrated" apart.
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

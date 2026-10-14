namespace ZBooksApi.DTOs;

public class UpdateBookDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public decimal Price { get; set; }
    public DateOnly PublicationDate { get; set; }
    public bool IsAvailable { get; set; }
}

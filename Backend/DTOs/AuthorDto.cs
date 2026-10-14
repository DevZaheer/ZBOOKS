namespace ZBooksApi.DTOs;

public class AuthorDto
{
    public int AuthorId { get; set; }
    public string Name { get; set; } = null!;
    public string? Biography { get; set; }
    public string? Photo { get; set; }
}

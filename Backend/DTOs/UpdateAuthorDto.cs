namespace ZBooksApi.DTOs;

public class UpdateAuthorDto
{
    public string Name { get; set; } = null!;
    public string? Biography { get; set; }
    public string? Photo { get; set; }
}

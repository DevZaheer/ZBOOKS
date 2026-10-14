namespace ZBooksApi.DTOs;

public class CreateAuthorDto
{
    public string Name { get; set; } = null!;
    public string? Biography { get; set; }
    public string? Photo { get; set; }
}

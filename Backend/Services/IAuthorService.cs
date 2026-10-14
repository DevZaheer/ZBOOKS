using ZBooksApi.DTOs;
namespace ZBooksApi.Services;
public interface IAuthorService
{ 
    Task<List<AuthorDto>> GetAuthorsAsync(); 
    Task<AuthorDto?> GetAuthorAsync(int id); 
    Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto dto); 
    Task<AuthorDto?> UpdateAuthorAsync(int id, UpdateAuthorDto dto); 
    Task<bool> DeleteAuthorAsync(int id); }

using ZBooksApi.DTOs;

namespace ZBooksApi.Services;

public interface IBookService
{
    Task<PagedResult<BookDto>> GetBooksAsync(BookQueryParams query);
    Task<BookDto?> GetBookAsync(int id);
    Task<BookDto> CreateBookAsync(CreateBookDto dto);
    Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto dto);
    Task<bool> DeleteBookAsync(int id);
    Task<BookDto?> AddAuthorAsync(int bookId, int authorId);
    Task<BookDto?> RemoveAuthorAsync(int bookId, int authorId);
    Task<BookDto?> AddCategoryAsync(int bookId, int categoryId);
    Task<BookDto?> RemoveCategoryAsync(int bookId, int categoryId);
}

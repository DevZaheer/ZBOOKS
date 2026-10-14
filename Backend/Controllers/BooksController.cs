using Microsoft.AspNetCore.Mvc;
using ZBooksApi.DTOs;
using ZBooksApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    // Example: GET /api/books?search=clean&categoryId=2&sortBy=rating&descending=true&page=1&pageSize=12
    // Returns { items, page, pageSize, totalCount, totalPages }, not a bare array.
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] BookQueryParams query)
    {
        var result = await _bookService.GetBooksAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await _bookService.GetBookAsync(id);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateBook(CreateBookDto dto)
    {
        var book = await _bookService.CreateBookAsync(dto);

        return CreatedAtAction(
            nameof(GetBook),
            new { id = book.BookId },
            book);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookDto dto)
    {
        var book = await _bookService.UpdateBookAsync(id, dto);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var deleted = await _bookService.DeleteBookAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{bookId}/authors/{authorId}")]
    public async Task<IActionResult> AddAuthor(int bookId, int authorId)
    {
        var book = await _bookService.AddAuthorAsync(bookId, authorId);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{bookId}/authors/{authorId}")]
    public async Task<IActionResult> RemoveAuthor(int bookId, int authorId)
    {
        var book = await _bookService.RemoveAuthorAsync(bookId, authorId);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{bookId}/categories/{categoryId}")]
    public async Task<IActionResult> AddCategory(int bookId, int categoryId)
    {
        var book = await _bookService.AddCategoryAsync(bookId, categoryId);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{bookId}/categories/{categoryId}")]
    public async Task<IActionResult> RemoveCategory(int bookId, int categoryId)
    {
        var book = await _bookService.RemoveCategoryAsync(bookId, categoryId);

        if (book == null)
            return NotFound();

        return Ok(book);
    }
}

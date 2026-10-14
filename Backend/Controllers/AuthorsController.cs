using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;

    public AuthorsController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuthors()
        => Ok(await _authorService.GetAuthorsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(int id)
    {
        var author = await _authorService.GetAuthorAsync(id);

        if (author == null)
            return NotFound();

        return Ok(author);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateAuthor(CreateAuthorDto dto)
    {
        var author = await _authorService.CreateAuthorAsync(dto);

        return CreatedAtAction(
            nameof(GetAuthor),
            new { id = author.AuthorId },
            author);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto dto)
    {
        var author = await _authorService.UpdateAuthorAsync(id, dto);

        if (author == null)
            return NotFound();

        return Ok(author);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        var deleted = await _authorService.DeleteAuthorAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
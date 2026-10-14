using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZBooksApi.DTOs;
using ZBooksApi.Services;
using System.Security.Claims;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChaptersController : ControllerBase
{
    private readonly IChapterService _chapterService;

    public ChaptersController(IChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetChapters([FromQuery] int? bookId)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var chapters = await _chapterService.GetChaptersAsync(userId, isAdmin, bookId);

        return Ok(chapters);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChapter(int id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        // UnauthorizedAccessException from the service is now translated to a
        // 403 by the global exception middleware (see Program.cs), so no
        // local try/catch is needed here anymore.
        var chapter = await _chapterService.GetChapterAsync(id, userId, isAdmin);

        if (chapter == null)
            return NotFound();

        return Ok(chapter);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateChapter(CreateChapterDto dto)
    {
        var chapter = await _chapterService.CreateChapterAsync(dto);

        return CreatedAtAction(
            nameof(GetChapter),
            new { id = chapter.ChapterId },
            chapter);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateChapter(int id, UpdateChapterDto dto)
    {
        var chapter = await _chapterService.UpdateChapterAsync(id, dto);

        if (chapter == null)
            return NotFound();

        return Ok(chapter);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChapter(int id)
    {
        var deleted = await _chapterService.DeleteChapterAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

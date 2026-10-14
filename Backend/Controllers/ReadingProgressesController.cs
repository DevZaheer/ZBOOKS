using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReadingProgressesController : ControllerBase
{
    private readonly IReadingProgressService _readingProgressService;

    public ReadingProgressesController(
        IReadingProgressService readingProgressService)
    {
        _readingProgressService = readingProgressService;
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetReadingProgresses()
    {
        var userId = GetUserId();

        var progress = await _readingProgressService
            .GetReadingProgressesAsync(userId);

        return Ok(progress);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReadingProgress(int id)
    {
        var userId = GetUserId();

        var progress = await _readingProgressService
            .GetReadingProgressAsync(id, userId);

        if (progress == null)
            return NotFound();

        return Ok(progress);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReadingProgress(
        CreateReadingProgressDto dto)
    {
        var userId = GetUserId();

        var progress = await _readingProgressService
            .CreateReadingProgressAsync(dto, userId);

        return CreatedAtAction(
            nameof(GetReadingProgress),
            new { id = progress.ReadingProgressId },
            progress);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReadingProgress(
        int id,
        UpdateReadingProgressDto dto)
    {
        var userId = GetUserId();

        var progress = await _readingProgressService
            .UpdateReadingProgressAsync(id, dto, userId);

        if (progress == null)
            return NotFound();

        return Ok(progress);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReadingProgress(int id)
    {
        var userId = GetUserId();

        var deleted = await _readingProgressService
            .DeleteReadingProgressAsync(id, userId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
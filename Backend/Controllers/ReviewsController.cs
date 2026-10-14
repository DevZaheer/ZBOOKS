using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _reviewService.GetReviewsAsync();

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReview(int id)
    {
        var review = await _reviewService.GetReviewAsync(id);

        if (review == null)
            return NotFound();

        return Ok(review);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReview(CreateReviewDto dto)
    {
        var userId = GetUserId();

        // "Not purchased" / "already reviewed" throw InvalidOperationException
        // in the service; the global exception middleware turns that into a
        // 400 with the message, so no local try/catch is needed here.
        var review = await _reviewService.CreateReviewAsync(dto, userId);

        return CreatedAtAction(
            nameof(GetReview),
            new { id = review.ReviewId },
            review);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto dto)
    {
        var userId = GetUserId();

        var review = await _reviewService.UpdateReviewAsync(id, dto, userId);

        if (review == null)
            return NotFound();

        return Ok(review);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = GetUserId();
        var isAdmin = User.IsInRole("Admin");

        var deleted = await _reviewService.DeleteReviewAsync(id, userId, isAdmin);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

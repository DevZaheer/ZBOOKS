using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistsController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistsController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlists()
    {
        var userId = GetUserId();

        var wishlists = await _wishlistService
            .GetWishlistsAsync(userId);

        return Ok(wishlists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWishlist(int id)
    {
        var userId = GetUserId();

        var wishlist = await _wishlistService
            .GetWishlistAsync(id, userId);

        if (wishlist == null)
            return NotFound();

        return Ok(wishlist);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWishlist(
        CreateWishlistDto dto)
    {
        var userId = GetUserId();

        var wishlist = await _wishlistService
            .CreateWishlistAsync(dto, userId);

        return CreatedAtAction(
            nameof(GetWishlist),
            new { id = wishlist.WishlistId },
            wishlist);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWishlist(int id)
    {
        var userId = GetUserId();

        var deleted = await _wishlistService
            .DeleteWishlistAsync(id, userId);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZBooksApi.DTOs;
using ZBooksApi.Services;
using System.Security.Claims;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPurchases()
    {
        var userId = GetUserId();

        var purchases = await _purchaseService
            .GetPurchasesAsync(userId);

        return Ok(purchases);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPurchase(int id)
    {
        var userId = GetUserId();

        var purchase = await _purchaseService
            .GetPurchaseAsync(id, userId);

        if (purchase == null)
            return NotFound();

        return Ok(purchase);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePurchase(CreatePurchaseDto dto)
    {
        var userId = GetUserId();

        // Book-not-found / not-available now throws InvalidOperationException,
        // which the global exception middleware turns into a 400 automatically.
        var purchase = await _purchaseService.CreatePurchaseAsync(dto, userId);

        return CreatedAtAction(
            nameof(GetPurchase),
            new { id = purchase.PurchaseId },
            purchase);
    }

    [HttpPost("batch")]
    [Authorize]
    public async Task<IActionResult> CreatePurchases(CreatePurchasesDto dto)
    {
        var purchases = await _purchaseService.CreatePurchasesAsync(dto, GetUserId());
        return Ok(purchases);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePurchase(
        int id,
        UpdatePurchaseDto dto)
    {
        var purchase = await _purchaseService
            .UpdatePurchaseAsync(id, dto);

        if (purchase == null)
            return NotFound();

        return Ok(purchase);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePurchase(int id)
    {
        var deleted = await _purchaseService
            .DeletePurchaseAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPayments()
    {
        var userId = GetUserId();

        var payments = await _paymentService
            .GetPaymentsAsync(userId);

        return Ok(payments);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetPayment(int id)
    {
        var userId = GetUserId();

        var payment = await _paymentService
            .GetPaymentAsync(id, userId);

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePayment(
        CreatePaymentDto dto)
    {
        var userId = GetUserId();

        try
        {
            var payment = await _paymentService
                .CreatePaymentAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetPayment),
                new { id = payment.PaymentId },
                payment);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("batch")]
    [Authorize]
    public async Task<IActionResult> CreatePayments(CreateBatchPaymentDto dto)
    {
        try
        {
            var payments = await _paymentService.CreatePaymentsAsync(dto, GetUserId());
            return Ok(payments);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePayment(
        int id,
        UpdatePaymentDto dto)
    {
        var payment = await _paymentService
            .UpdatePaymentAsync(id, dto);

        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        var deleted = await _paymentService
            .DeletePaymentAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
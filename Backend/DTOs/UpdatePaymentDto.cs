namespace ZBooksApi.DTOs;

public class UpdatePaymentDto
{
    public string? PaymentMethod { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? PaidAt { get; set; }
}

namespace ZBooksApi.DTOs;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public int PurchaseId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentReference { get; set; } = null!;
    public string? PaymentMethod { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? PaidAt { get; set; }
}

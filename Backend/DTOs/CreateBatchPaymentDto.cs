namespace ZBooksApi.DTOs;

public class CreateBatchPaymentDto
{
    public List<int> PurchaseIds { get; set; } = [];
    public string PaymentReference { get; set; } = null!;
    public string? PaymentMethod { get; set; }
}

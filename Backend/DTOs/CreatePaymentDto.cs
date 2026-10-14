namespace ZBooksApi.DTOs;

// Amount and Status are deliberately NOT here: the server takes the amount
// from the purchase and decides the status itself.
public class CreatePaymentDto
{
    public int PurchaseId { get; set; }
    public string PaymentReference { get; set; } = null!;
    public string? PaymentMethod { get; set; }
}

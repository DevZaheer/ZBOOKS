namespace ZBooksApi.DTOs;

public class PurchaseDto
{
    public int PurchaseId { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PurchasedAt { get; set; }
    public string Status { get; set; } = null!;
}

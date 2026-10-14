using System;
using System.Collections.Generic;

namespace ZBooksApi.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int PurchaseId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentReference { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PaidAt { get; set; }

    public virtual Purchase Purchase { get; set; } = null!;
}

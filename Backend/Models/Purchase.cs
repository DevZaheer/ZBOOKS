using System;
using System.Collections.Generic;

namespace ZBooksApi.Models;

public partial class Purchase
{
    public int PurchaseId { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PurchasedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
using System;
using System.Collections.Generic;

namespace ZBooksApi.Models;

public partial class ReadingProgress
{
    public int ReadingProgressId { get; set; }

    public int UserId { get; set; }

    public int BookId { get; set; }

    public int CurrentChapter { get; set; }

    public int CurrentPage { get; set; }

    public decimal ProgressPercentage { get; set; }

    public DateTime LastReadAt { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

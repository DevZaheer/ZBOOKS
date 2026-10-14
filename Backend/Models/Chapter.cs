using System;
using System.Collections.Generic;

namespace ZBooksApi.Models;

public partial class Chapter
{
    public int ChapterId { get; set; }

    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public int ChapterNumber { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Book Book { get; set; } = null!;
}

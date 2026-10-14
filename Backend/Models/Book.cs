using System;
using System.Collections.Generic;

namespace ZBooksApi.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? CoverImage { get; set; }

    public decimal Price { get; set; }

    public DateOnly PublicationDate { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<ReadingProgress> ReadingProgresses { get; set; } = new List<ReadingProgress>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}

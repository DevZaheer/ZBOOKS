namespace ZBooksApi.DTOs;

// Bound from the query string, e.g.
//   GET /api/books?search=clean&categoryId=2&minPrice=1000&sortBy=price&page=1&pageSize=12
// Every property is optional and has a safe default.
public class BookQueryParams
{
    // Matches against title, description and author name.
    public string? Search { get; set; }

    public int? CategoryId { get; set; }
    public int? AuthorId { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // null = both available and unavailable books.
    public bool? IsAvailable { get; set; }

    // One of: title | price | newest | rating. Anything else falls back to title.
    public string SortBy { get; set; } = "title";

    // For "newest" you want sortBy=newest&descending=true (newest first).
    public bool Descending { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}

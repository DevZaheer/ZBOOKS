using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class BookService : IBookService
{
    private const int MaxPageSize = 50;

    private readonly ZBooksDbContext _context;

    public BookService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BookDto>> GetBooksAsync(BookQueryParams p)
    {
        // Never trust paging numbers from the client: a pageSize of 1,000,000
        // would pull the whole table in one request.
        var page = Math.Max(p.Page, 1);
        var pageSize = Math.Clamp(p.PageSize, 1, MaxPageSize);

        // Build the query up step by step. Nothing hits the database until
        // CountAsync / ToListAsync, so all of these Where calls are merged
        // into ONE SQL statement.
        var query = _context.Books.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            var term = p.Search.Trim();

            query = query.Where(b =>
                b.Title.Contains(term) ||
                (b.Description != null && b.Description.Contains(term)) ||
                b.Authors.Any(a => a.Name.Contains(term)));
        }

        if (p.CategoryId.HasValue)
        {
            var categoryId = p.CategoryId.Value;
            query = query.Where(b => b.Categories.Any(c => c.CategoryId == categoryId));
        }

        if (p.AuthorId.HasValue)
        {
            var authorId = p.AuthorId.Value;
            query = query.Where(b => b.Authors.Any(a => a.AuthorId == authorId));
        }

        if (p.MinPrice.HasValue)
        {
            var minPrice = p.MinPrice.Value;
            query = query.Where(b => b.Price >= minPrice);
        }

        if (p.MaxPrice.HasValue)
        {
            var maxPrice = p.MaxPrice.Value;
            query = query.Where(b => b.Price <= maxPrice);
        }

        if (p.IsAvailable.HasValue)
        {
            var isAvailable = p.IsAvailable.Value;
            query = query.Where(b => b.IsAvailable == isAvailable);
        }

        // Count BEFORE Skip/Take, otherwise TotalCount would just be the page size.
        var totalCount = await query.CountAsync();

        var ordered = ApplySorting(query, p.SortBy, p.Descending);

        var items = await ProjectToDto(
                ordered.Skip((page - 1) * pageSize).Take(pageSize))
            .ToListAsync();

        foreach (var item in items)
            RoundRating(item);

        return new PagedResult<BookDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<BookDto?> GetBookAsync(int id)
    {
        var book = await ProjectToDto(
                _context.Books.AsNoTracking().Where(b => b.BookId == id))
            .FirstOrDefaultAsync();

        if (book != null)
            RoundRating(book);

        return book;
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
    {
        var entity = new Book
        {
            Title = dto.Title,
            Description = dto.Description,
            CoverImage = dto.CoverImage,
            Price = dto.Price,
            PublicationDate = dto.PublicationDate,
            IsAvailable = dto.IsAvailable,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Books.Add(entity);
        await _context.SaveChangesAsync();

        return (await GetBookAsync(entity.BookId))!;
    }

    public async Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto dto)
    {
        var entity = await _context.Books.FindAsync(id);
        if (entity == null) return null;

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.CoverImage = dto.CoverImage;
        entity.Price = dto.Price;
        entity.PublicationDate = dto.PublicationDate;
        entity.IsAvailable = dto.IsAvailable;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetBookAsync(id);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var entity = await _context.Books.FindAsync(id);
        if (entity == null) return false;

        _context.Books.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BookDto?> AddAuthorAsync(int bookId, int authorId)
    {
        var book = await _context.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.BookId == bookId);

        var author = await _context.Authors.FindAsync(authorId);

        if (book == null || author == null) return null;

        if (!book.Authors.Any(a => a.AuthorId == authorId))
            book.Authors.Add(author);

        await _context.SaveChangesAsync();
        return await GetBookAsync(bookId);
    }

    public async Task<BookDto?> RemoveAuthorAsync(int bookId, int authorId)
    {
        var book = await _context.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.BookId == bookId);

        if (book == null) return null;

        var author = book.Authors.FirstOrDefault(a => a.AuthorId == authorId);
        if (author != null)
            book.Authors.Remove(author);

        await _context.SaveChangesAsync();
        return await GetBookAsync(bookId);
    }

    public async Task<BookDto?> AddCategoryAsync(int bookId, int categoryId)
    {
        var book = await _context.Books
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.BookId == bookId);

        var category = await _context.Categories.FindAsync(categoryId);

        if (book == null || category == null) return null;

        if (!book.Categories.Any(c => c.CategoryId == categoryId))
            book.Categories.Add(category);

        await _context.SaveChangesAsync();
        return await GetBookAsync(bookId);
    }

    public async Task<BookDto?> RemoveCategoryAsync(int bookId, int categoryId)
    {
        var book = await _context.Books
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.BookId == bookId);

        if (book == null) return null;

        var category = book.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
        if (category != null)
            book.Categories.Remove(category);

        await _context.SaveChangesAsync();
        return await GetBookAsync(bookId);
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private static IOrderedQueryable<Book> ApplySorting(
        IQueryable<Book> query, string? sortBy, bool descending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "price" => OrderBy(query, b => b.Price, descending),
            "newest" => OrderBy(query, b => b.CreatedAt, descending),

            // Average over zero reviews is NULL in SQL, so unrated books sort
            // first ascending and last descending (highest rated first).
            "rating" => OrderBy(query, b => b.Reviews.Average(r => (double?)r.Rating), descending),

            _ => OrderBy(query, b => b.Title, descending)
        };
    }

    // The ThenBy(BookId) tie-breaker matters for paging: without a fully
    // deterministic order, two books with the same price can swap places
    // between page 1 and page 2, so a book shows up twice or not at all.
    private static IOrderedQueryable<Book> OrderBy<TKey>(
        IQueryable<Book> query,
        Expression<Func<Book, TKey>> key,
        bool descending)
    {
        var ordered = descending
            ? query.OrderByDescending(key)
            : query.OrderBy(key);

        return ordered.ThenBy(b => b.BookId);
    }

    // One shared projection, so the list, the detail page, create and update
    // all return the same shape. EF turns this into SQL, including the
    // AVG / COUNT over reviews, so no review rows are loaded into memory.
    private static IQueryable<BookDto> ProjectToDto(IQueryable<Book> query) =>
        query.Select(b => new BookDto
        {
            BookId = b.BookId,
            Title = b.Title,
            Description = b.Description,
            CoverImage = b.CoverImage,
            Price = b.Price,
            PublicationDate = b.PublicationDate,
            IsAvailable = b.IsAvailable,
            AuthorIds = b.Authors.Select(a => a.AuthorId).ToList(),
            CategoryIds = b.Categories.Select(c => c.CategoryId).ToList(),
            AverageRating = b.Reviews.Average(r => (double?)r.Rating) ?? 0,
            ReviewCount = b.Reviews.Count()
        });

    private static void RoundRating(BookDto dto) =>
        dto.AverageRating = Math.Round(dto.AverageRating, 1);
}

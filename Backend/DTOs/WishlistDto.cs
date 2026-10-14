namespace ZBooksApi.DTOs;

public class WishlistDto
{
    public int WishlistId { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateTime AddedAt { get; set; }
}

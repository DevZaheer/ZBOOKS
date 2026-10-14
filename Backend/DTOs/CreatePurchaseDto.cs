namespace ZBooksApi.DTOs;

public class CreatePurchaseDto
{
    public int BookId { get; set; }
}

public class CreatePurchasesDto
{
    public List<int> BookIds { get; set; } = [];
}

using ZBooksApi.DTOs;
namespace ZBooksApi.Services;
public interface IReviewService
{ 
     Task<List<ReviewDto>> GetReviewsAsync(); 
     Task<ReviewDto?> GetReviewAsync(int id);
     Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto,int userId);
     Task<ReviewDto?> UpdateReviewAsync(int id,UpdateReviewDto dto,int userId);
     Task<bool> DeleteReviewAsync(int id,int userId,bool isAdmin); }

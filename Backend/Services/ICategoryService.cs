using ZBooksApi.DTOs;
namespace ZBooksApi.Services;
public interface ICategoryService
{ 
     Task<List<CategoryDto>> GetCategoriesAsync(); 
     Task<CategoryDto?> GetCategoryAsync(int id); 
     Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto); 
     Task<CategoryDto?> UpdateCategoryAsync(int id,UpdateCategoryDto dto); 
     Task<bool> DeleteCategoryAsync(int id); }

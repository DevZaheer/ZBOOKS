using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class CategoryService : ICategoryService
{
    private readonly ZBooksDbContext _context;

    public CategoryService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        return await _context.Categories
            .Select(e => new CategoryDto
            {
                CategoryId = e.CategoryId,
                Name = e.Name,
                Description = e.Description
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetCategoryAsync(int id)
    {
        return await _context.Categories
            .Where(e => e.CategoryId == id)
            .Select(e => new CategoryDto
            {
                CategoryId = e.CategoryId,
                Name = e.Name,
                Description = e.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var entity = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Description = dto.Description;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var entity = await _context.Categories.FindAsync(id);
        if (entity == null) return false;

        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CategoryDto ToDto(Category e) => new()
    {
        CategoryId = e.CategoryId,
        Name = e.Name,
        Description = e.Description
    };
}

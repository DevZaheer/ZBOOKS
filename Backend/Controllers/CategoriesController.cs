using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
        => Ok(await _categoryService.GetCategoriesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryAsync(id);

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateCategoryAsync(dto);

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.CategoryId },
            category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryService.UpdateCategoryAsync(id, dto);

        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var deleted = await _categoryService.DeleteCategoryAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
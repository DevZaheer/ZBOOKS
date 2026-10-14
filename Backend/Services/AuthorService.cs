using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class AuthorService : IAuthorService
{
    private readonly ZBooksDbContext _context;

    public AuthorService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuthorDto>> GetAuthorsAsync()
    {
        return await _context.Authors
            .Select(e => new AuthorDto
            {
                AuthorId = e.AuthorId,
                Name = e.Name,
                Biography = e.Biography,
                Photo = e.Photo
            })
            .ToListAsync();
    }

    public async Task<AuthorDto?> GetAuthorAsync(int id)
    {
        return await _context.Authors
            .Where(e => e.AuthorId == id)
            .Select(e => new AuthorDto
            {
                AuthorId = e.AuthorId,
                Name = e.Name,
                Biography = e.Biography,
                Photo = e.Photo
            })
            .FirstOrDefaultAsync();
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto dto)
    {
        var entity = new Author
        {
            Name = dto.Name,
            Biography = dto.Biography,
            Photo = dto.Photo
        };

        _context.Authors.Add(entity);
        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<AuthorDto?> UpdateAuthorAsync(int id, UpdateAuthorDto dto)
    {
        var entity = await _context.Authors.FindAsync(id);
        if (entity == null) return null;

        entity.Name = dto.Name;
        entity.Biography = dto.Biography;
        entity.Photo = dto.Photo;

        await _context.SaveChangesAsync();
        return ToDto(entity);
    }

    public async Task<bool> DeleteAuthorAsync(int id)
    {
        var entity = await _context.Authors.FindAsync(id);
        if (entity == null) return false;

        _context.Authors.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static AuthorDto ToDto(Author e) => new()
    {
        AuthorId = e.AuthorId,
        Name = e.Name,
        Biography = e.Biography,
        Photo = e.Photo
    };
}

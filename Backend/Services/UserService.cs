using Microsoft.EntityFrameworkCore;
using ZBooksApi.DTOs;
using ZBooksApi.Models;

namespace ZBooksApi.Services;

public class UserService : IUserService
{
    private readonly ZBooksDbContext _context;

    public UserService(ZBooksDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _context.Users
            .Select(entity => new UserDto
            {
                UserId = entity.UserId,
                FullName = entity.FullName,
                Email = entity.Email,
                Role = entity.Role,
                CreatedAt = entity.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<UserDto?> GetUserAsync(int id)
    {
        return await _context.Users
            .Where(entity => entity.UserId == id)
            .Select(entity => new UserDto
            {
                UserId = entity.UserId,
                FullName = entity.FullName,
                Email = entity.Email,
                Role = entity.Role,
                CreatedAt = entity.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

   

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto)
    {
        var entity = await _context.Users.FindAsync(id);

        if (entity == null)
            return null;

        entity.FullName = dto.FullName;
        entity.Email = dto.Email;
        entity.Role = dto.Role;

        await _context.SaveChangesAsync();

        return new UserDto
        {
                UserId = entity.UserId,
                FullName = entity.FullName,
                Email = entity.Email,
                Role = entity.Role,
                CreatedAt = entity.CreatedAt
        };
    }
    
    public async Task<bool> DeleteUserAsync(int id)
    {
        var entity = await _context.Users.FindAsync(id);

        if (entity == null)
            return false;

        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();

        return true;
    }
   

   
}
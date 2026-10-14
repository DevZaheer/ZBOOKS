using ZBooksApi.Models;
using ZBooksApi.DTOs;

namespace ZBooksApi.Services;

public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserAsync(int id);
    
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
   
}
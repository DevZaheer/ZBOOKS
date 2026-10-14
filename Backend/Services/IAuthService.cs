using ZBooksApi.DTOs;

namespace ZBooksApi.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<string?> LoginAsync(LoginDto dto);
}
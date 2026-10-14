using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZBooksApi.DTOs;
using ZBooksApi.Models;
namespace ZBooksApi.Services;

public class AuthService : IAuthService
{
    private readonly ZBooksDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

   public AuthService(
    ZBooksDbContext context,
    IConfiguration configuration)
   {
    _context = context;
    _configuration = configuration;
    _passwordHasher = new PasswordHasher<User>();
   }

    public async Task<UserDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (existingUser != null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            FullName = dto.Username,
            Email = dto.Email,
            Role = "Reader",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

   public async Task<string?> LoginAsync(LoginDto dto)
   {
    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null)
        return null;

    var result = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        dto.Password);

    if (result == PasswordVerificationResult.Failed)
        return null;

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"]!));

    var credentials = new SigningCredentials(
        key,
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
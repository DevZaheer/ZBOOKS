using Microsoft.AspNetCore.Mvc;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var user = await _authService.RegisterAsync(dto);

            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto);

        if (token == null)
            return Unauthorized("Invalid email or password.");

        return Ok(new
        {
            token
        });
    }
}
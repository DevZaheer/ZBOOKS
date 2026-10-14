using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZBooksApi.DTOs;
using ZBooksApi.Services;

namespace ZBooksApi.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
        => Ok(await _userService.GetUsersAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(
        int id,
        UpdateUserDto dto)
    {
        var user = await _userService.UpdateUserAsync(id, dto);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userService.DeleteUserAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
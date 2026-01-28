using Application.DTOs.User;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();
        var profile = await _userService.GetProfileByUsernameAsync(username);
        if (profile == null)
            return NotFound(new { message = "User not found" });
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        try
        {
            var profile = await _userService.UpdateProfileAsync(userId, request);
            if (profile == null) return NotFound();
            return Ok(profile);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("user-only")]
    [Authorize(Policy = "UserOrAbove")]
    public IActionResult UserOnly()
    {
        return Ok(new { message = "This endpoint is accessible to User, Manager, and Admin" });
    }

    [HttpGet("manager-only")]
    [Authorize(Policy = "ManagerOrAdmin")]
    public IActionResult ManagerOnly()
    {
        return Ok(new { message = "This endpoint is accessible to Manager and Admin only" });
    }

    [HttpGet("admin-only")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AdminOnly()
    {
        return Ok(new { message = "This endpoint is accessible to Admin only" });
    }
}


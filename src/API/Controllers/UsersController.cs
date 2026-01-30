using Application.DTOs.User;
using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for user profile management (authenticated users only)
/// </summary>
[ApiController]
[Route("api/user")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IAuthService authService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>User profile information</returns>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized();
            
            var profile = await _userService.GetProfileByUsernameAsync(username);
            if (profile == null)
                return NotFound(new { message = "User not found" });
            
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile for username: {Username}", User.Identity?.Name);
            return StatusCode(500, new { message = "An error occurred while retrieving profile" });
        }
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="request">Profile update request</param>
    /// <returns>Updated profile information</returns>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserProfileResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();
            
            // Validate request
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Name is required" });
            
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email is required" });
            
            var profile = await _userService.UpdateProfileAsync(userId, request);
            if (profile == null) 
                return NotFound(new { message = "User not found" });
            
            return Ok(profile);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for user ID: {UserId}", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { message = "An error occurred while updating profile" });
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

    /// <summary>
    /// Change current user password
    /// </summary>
    /// <param name="request">Password change request</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // Validate request
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return BadRequest(new { message = "Current password is required" });
            
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
                return BadRequest(new { message = "New password must be at least 8 characters" });

            await _authService.ChangePasswordAsync(userId, request);
            return Ok(new { message = "Password changed successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user ID: {UserId}", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { message = "An error occurred while changing password" });
        }
    }

    /// <summary>
    /// Get user statistics and activity summary
    /// </summary>
    /// <returns>User statistics</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUserStats()
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // TODO: Implement user statistics service
            // This would typically return:
            // - Total practice sessions
            // - Total submissions
            // - Average scores
            // - Most practiced topics
            // - Progress by level
            
            return Ok(new { 
                message = "User statistics endpoint - implementation pending",
                userId = userId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user stats for user ID: {UserId}", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { message = "An error occurred while retrieving user statistics" });
        }
    }

    /// <summary>
    /// Get user practice history
    /// </summary>
    /// <param name="limit">Number of recent sessions to return (default: 10, max: 50)</param>
    /// <returns>User practice history</returns>
    [HttpGet("practice-history")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetPracticeHistory([FromQuery] int limit = 10)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            // Validate limit parameter
            if (limit < 1) limit = 10;
            if (limit > 50) limit = 50;

            // TODO: Implement practice history retrieval
            // This would typically return:
            // - Recent practice sessions
            // - Session details (mode, date, duration)
            // - Submission counts per session
            // - Performance metrics
            
            return Ok(new { 
                message = "Practice history endpoint - implementation pending",
                userId = userId,
                limit = limit
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving practice history for user ID: {UserId}", User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { message = "An error occurred while retrieving practice history" });
        }
    }
}


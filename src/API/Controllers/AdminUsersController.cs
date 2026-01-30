using Application.DTOs.User;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Admin-only controller for user management operations
/// </summary>
[ApiController]
[Route("api/admin/users")]
[Authorize(Policy = "AdminOnly")]
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AdminUsersController> _logger;

    public AdminUsersController(
        IUserService userService,
        ILogger<AdminUsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get all users with pagination, search, and sorting
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
    /// <param name="search">Search term for name, username, or email</param>
    /// <param name="sortBy">Field to sort by (id, name, username, email, role, createdat)</param>
    /// <param name="sortDesc">Sort in descending order (default: false)</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserListResponse>), 200)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false)
    {
        try
        {
            // Validate parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var users = await _userService.GetAllUsersAsync(page, pageSize, search, sortBy, sortDesc);
            var totalCount = await _userService.GetTotalUserCountAsync();
            
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            Response.Headers.Append("X-Total-Pages", totalPages.ToString());
            Response.Headers.Append("X-Current-Page", page.ToString());
            Response.Headers.Append("X-Page-Size", pageSize.ToString());

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, new { message = "An error occurred while retrieving users" });
        }
    }

    /// <summary>
    /// Get user details by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AdminUserResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdForAdminAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });
            
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving user" });
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation request</param>
    /// <returns>Created user details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AdminUserResponse), 201)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { message = "Name is required" });
            
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest(new { message = "Username is required" });
            
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email is required" });
            
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                return BadRequest(new { message = "Password must be at least 8 characters" });

            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { message = "An error occurred while creating user" });
        }
    }

    /// <summary>
    /// Update user information
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Update request</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(AdminUserResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, request);
            if (user == null)
                return NotFound(new { message = "User not found" });
            
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating user" });
        }
    }

    /// <summary>
    /// Update user password
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Password update request</param>
    /// <returns>Success status</returns>
    [HttpPut("{id:int}/password")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> UpdateUserPassword(int id, [FromBody] UpdateUserPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
                return BadRequest(new { message = "Password must be at least 8 characters" });

            var success = await _userService.UpdateUserPasswordAsync(id, request);
            if (!success)
                return NotFound(new { message = "User not found" });
            
            return Ok(new { message = "Password updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating password for user ID {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating password" });
        }
    }

    /// <summary>
    /// Delete a user (cannot delete admin users)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success)
                return NotFound(new { message = "User not found" });
            
            return Ok(new { message = "User deleted successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting user" });
        }
    }

    /// <summary>
    /// Toggle user active status (activate/deactivate)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success status with new status</returns>
    [HttpPatch("{id:int}/toggle-status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        try
        {
            var success = await _userService.ToggleUserStatusAsync(id);
            if (!success)
                return NotFound(new { message = "User not found" });
            
            // Get updated user to return current status
            var user = await _userService.GetUserByIdForAdminAsync(id);
            return Ok(new { 
                message = "User status updated successfully",
                isActive = user?.IsActive
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling status for user ID {UserId}", id);
            return StatusCode(500, new { message = "An error occurred while updating user status" });
        }
    }

    /// <summary>
    /// Get total user count
    /// </summary>
    /// <returns>Total number of users</returns>
    [HttpGet("count")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetUserCount()
    {
        try
        {
            var count = await _userService.GetTotalUserCountAsync();
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user count");
            return StatusCode(500, new { message = "An error occurred while retrieving user count" });
        }
    }
}
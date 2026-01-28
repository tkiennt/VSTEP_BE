using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public AuthController(
        IAuthService authService, 
        ILogger<AuthController> logger,
        IValidator<RegisterRequest> registerValidator)
    {
        _authService = authService;
        _logger = logger;
        _registerValidator = registerValidator;
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 401)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Failed login attempt for username: {Username}", request.Username);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for username: {Username}", request.Username);
            return BadRequest(new { message = "An error occurred during login. Please try again." });
        }
    }

    /// <summary>
    /// User registration endpoint with comprehensive validation
    /// </summary>
    /// <remarks>
    /// All new registrations are automatically assigned the "User" role.
    /// Only administrators can change user roles through admin endpoints.
    /// 
    /// Validation Rules:
    /// - Name: 2-100 characters, letters only
    /// - Username: 3-50 characters, alphanumeric with _ and -, must be unique
    /// - Email: Valid email format, max 150 characters, must be unique
    /// - Phone: Vietnamese format (0912345678 or +84912345678) - optional
    /// - Password: Min 8 chars, uppercase, lowercase, digit, special character
    /// 
    /// Note: Username and email uniqueness are validated asynchronously before database operations.
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(object), 400)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Manual validation to support async rules (username/email uniqueness)
            var validationResult = await _registerValidator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
            {
                // Return validation errors in standard format
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                
                return BadRequest(new 
                { 
                    type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors 
                });
            }

            var response = await _authService.RegisterAsync(request);
            
            _logger.LogInformation("User registered successfully: {Username}, Email: {Email}", 
                request.Username, request.Email);
            
            return Ok(response);
        }
        catch (DbUpdateException ex)
        {
            // Database constraint violations (rare, as validation happens first)
            _logger.LogError(ex, "Database error during registration. Username: {Username}, Email: {Email}. InnerException: {InnerException}", 
                request.Username, request.Email, ex.InnerException?.Message);
            
            // Extract meaningful error message
            var errorMessage = ExtractDbErrorMessage(ex);
            return BadRequest(new { message = errorMessage });
        }
        catch (Exception ex)
        {
            // Unexpected errors
            _logger.LogError(ex, "Unexpected error during registration. Username: {Username}, Email: {Email}. InnerException: {InnerException}", 
                request.Username, request.Email, ex.InnerException?.Message);
            
            return StatusCode(500, new { message = "An unexpected error occurred during registration. Please try again later." });
        }
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult Logout()
    {
        // JWT is stateless, so logout is handled on the client side by removing the token
        // Optionally, you can implement token blacklisting here if needed
        return Ok(new { message = "Logged out successfully. Please remove the token from client storage." });
    }

    [HttpPost("validate")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> ValidateToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var isValid = await _authService.ValidateTokenAsync(token);
        return Ok(new { isValid });
    }

    /// <summary>Request password reset. Token is sent by email in production; for testing check DB password_reset_tokens.</summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request);
        return Ok(new { message = "If the email exists, a reset link has been sent." });
    }

    /// <summary>Reset password using token from forgot-password email (or from DB for testing).</summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            await _authService.ResetPasswordAsync(request);
            return Ok(new { message = "Password has been reset successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Change password for the currently logged-in user.</summary>
    [HttpPost("change-password")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        try
        {
            await _authService.ChangePasswordAsync(userId, request);
            return Ok(new { message = "Password changed successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Extracts meaningful error messages from DbUpdateException
    /// Common causes of DbUpdateException:
    /// 1. Unique constraint violations (duplicate username/email)
    /// 2. Foreign key constraint violations
    /// 3. Required field violations (NOT NULL)
    /// 4. Data type mismatches
    /// 5. String length violations
    /// </summary>
    private string ExtractDbErrorMessage(DbUpdateException ex)
    {
        var innerException = ex.InnerException?.Message ?? ex.Message;

        // MySQL specific error messages
        if (innerException.Contains("Duplicate entry") || innerException.Contains("UNIQUE"))
        {
            if (innerException.Contains("username"))
                return "Username already exists. Please choose a different username.";
            if (innerException.Contains("email"))
                return "Email already exists. Please use a different email address.";
            if (innerException.Contains("phone"))
                return "Phone number already exists. Please use a different phone number.";
            
            return "A record with this information already exists.";
        }

        if (innerException.Contains("foreign key constraint", StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid reference data provided. Please check your input.";
        }

        if (innerException.Contains("cannot be null", StringComparison.OrdinalIgnoreCase))
        {
            return "Required field is missing. Please check your input.";
        }

        if (innerException.Contains("Data too long", StringComparison.OrdinalIgnoreCase))
        {
            return "One or more fields exceed the maximum allowed length.";
        }

        // Default error message for production (don't expose internal details)
        return "Unable to save changes. Please verify your input and try again.";
    }
}


using Domain.Enums;

namespace Application.DTOs.User;

/// <summary>
/// Request DTO for creating a new user (admin only)
/// </summary>
public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public Role Role { get; set; } = Role.User;
    public int? TargetLevelId { get; set; }
    public bool IsActive { get; set; } = true;
}
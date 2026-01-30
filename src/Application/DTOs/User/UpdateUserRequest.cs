using Domain.Enums;

namespace Application.DTOs.User;

/// <summary>
/// Request DTO for updating user information (admin only)
/// </summary>
public class UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Role Role { get; set; }
    public int? TargetLevelId { get; set; }
    public bool IsActive { get; set; }
}